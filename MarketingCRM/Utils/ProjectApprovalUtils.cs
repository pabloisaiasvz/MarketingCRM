using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MarketingCRM.Data;
using MarketingCRM.Models;
using System.Collections.Generic;

namespace MarketingCRM.Utils
{
    public static class ProjectApprovalUtils
    {
        public static void CreateProjectProposal(ProjectApprovalDbContext context, string title, string description, decimal estimatedAmount, int duration, int areaId, int projectTypeId, int userId)
        {
            var pendingStatus = context.ApprovalStatuses.FirstOrDefault(s => s.Name == "Pending");
            if (pendingStatus == null)
            {
                Console.WriteLine("Error: No se encontró el estado 'Pending'.");
                return;
            }

            var area = context.Areas.FirstOrDefault(a => a.Id == areaId);
            var projectType = context.ProjectTypes.FirstOrDefault(t => t.Id == projectTypeId);

            if (area == null || projectType == null)
            {
                Console.WriteLine("Error: Área o tipo de proyecto no válidos.");
                return;
            }

            var projectProposal = new ProjectProposal
            {
                Title = title,
                Description = description,
                EstimatedAmount = estimatedAmount,
                EstimatedDuration = duration,
                AreaId = areaId,
                TypeId = projectTypeId,
                Status = pendingStatus,
                StatusId = pendingStatus.Id,
                Area = area,
                Type = projectType,
                CreateAt = DateTime.Now,
                CreateById = userId
            };

            try
            {
                context.ProjectProposals.Add(projectProposal);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la propuesta: {ex.Message}");
                return;
            }

            var rules = GetApprovalRules(context, estimatedAmount, areaId, projectTypeId).ToList();

            if (!rules.Any())
            {
                Console.WriteLine("Advertencia: No se encontraron reglas de aprobación aplicables para este proyecto.");
                return;
            }

            foreach (var rule in rules)
            {
                var step = new ProjectApprovalStep
                {
                    ProjectProposalId = projectProposal.Id,
                    ProjectProposal = projectProposal,
                    ApproverRoleId = rule.ApproverRoleId,
                    StatusId = pendingStatus.Id,
                    Status = pendingStatus,
                    StepOrder = rule.StepOrder,
                    ApproverRole = context.ApproverRoles.FirstOrDefault(r => r.Id == rule.ApproverRoleId)
                };
                context.ProjectApprovalSteps.Add(step);
            }

            try
            {
                context.SaveChanges();
                Console.WriteLine("Propuesta de proyecto y pasos de aprobación creados con éxito!");
                Console.WriteLine($"ID del proyecto: {projectProposal.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar los pasos de aprobación: {ex.Message}");
            }
        }

        public static List<ApprovalRule> GetApprovalRules(ProjectApprovalDbContext context, decimal estimatedAmount, int areaId, int projectTypeId)
        {
            var applicableRules = context.ApprovalRules
                .Where(r =>
                    (r.MinAmount <= estimatedAmount) &&
                    (r.MaxAmount == 0 || r.MaxAmount >= estimatedAmount) &&
                    (r.AreaId == null || r.AreaId == areaId) &&
                    (r.TypeId == null || r.TypeId == projectTypeId))
                .ToList();

            var resultRules = new List<ApprovalRule>();
            var stepGroups = applicableRules.GroupBy(r => r.StepOrder);

            foreach (var group in stepGroups.OrderBy(g => g.Key))
            {
                if (group.Count() > 1)
                {
                    var mostSpecificRule = group.OrderByDescending(r =>
                        (r.AreaId.HasValue ? 2 : 0) +
                        (r.TypeId.HasValue ? 2 : 0) +
                        ((r.MaxAmount > 0 && r.MinAmount > 0) ? 1 : 0))
                        .First();

                    resultRules.Add(mostSpecificRule);
                }
                else
                {
                    resultRules.Add(group.First());
                }
            }

            return resultRules.OrderBy(r => r.StepOrder).ToList();
        }

        public static void GenerateApprovalSteps(ProjectApprovalDbContext context, decimal estimatedAmount, int areaId, int projectTypeId)
        {
            var approvalRules = GetApprovalRules(context, estimatedAmount, areaId, projectTypeId).ToList();

            if (!approvalRules.Any())
            {
                Console.WriteLine("No se encontraron reglas de aprobación aplicables.");
                return;
            }

            Console.WriteLine("\nPasos de Aprobación generados:");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("| Paso | Rol Aprobador |");
            Console.WriteLine("--------------------------------------");

            foreach (var rule in approvalRules)
            {
                var approverRole = context.ApproverRoles.FirstOrDefault(r => r.Id == rule.ApproverRoleId);
                string roleName = approverRole?.Name ?? "Rol desconocido";
                Console.WriteLine($"| {rule.StepOrder} | {roleName} |");
            }
            Console.WriteLine("--------------------------------------");
        }

        public static void ProcessApproval(ProjectApprovalDbContext context, Guid projectId, bool isApproved, int stepOrder, int userId)
        {
            var projectProposal = context.ProjectProposals
                .Include(p => p.Status)
                .FirstOrDefault(p => p.Id == projectId);

            if (projectProposal == null)
            {
                Console.WriteLine("Error: Proyecto no encontrado.");
                return;
            }

            var currentStep = context.ProjectApprovalSteps
                .Include(s => s.Status)
                .FirstOrDefault(s => s.ProjectProposalId == projectId && s.StepOrder == stepOrder);

            if (currentStep == null)
            {
                Console.WriteLine("Error: Paso de aprobación no encontrado.");
                return;
            }

            var statusApproved = context.ApprovalStatuses.FirstOrDefault(s => s.Name == "Approved");
            var statusRejected = context.ApprovalStatuses.FirstOrDefault(s => s.Name == "Rejected");

            if (statusApproved == null || statusRejected == null)
            {
                Console.WriteLine("Error: Estados 'Approved' o 'Rejected' no encontrados.");
                return;
            }

            currentStep.ApproverUserId = userId;

            if (isApproved)
            {
                currentStep.StatusId = statusApproved.Id;
                currentStep.Status = statusApproved;
                currentStep.DecisionDate = DateTime.Now;

                var nextStep = context.ProjectApprovalSteps
                    .Where(s => s.ProjectProposalId == projectId && s.StepOrder > stepOrder)
                    .OrderBy(s => s.StepOrder)
                    .FirstOrDefault();

                if (nextStep == null)
                {
                    var allApproved = context.ProjectApprovalSteps
                        .Where(s => s.ProjectProposalId == projectId)
                        .All(s => s.StatusId == statusApproved.Id);

                    if (allApproved)
                    {
                        projectProposal.Status = statusApproved;
                        projectProposal.StatusId = statusApproved.Id;
                        Console.WriteLine("¡El proyecto ha sido aprobado completamente!");
                    }
                    else
                    {
                        Console.WriteLine("Este paso fue aprobado, pero aún faltan otros pasos por aprobar.");
                    }
                }
                else
                {
                    var nextRole = context.ApproverRoles.FirstOrDefault(r => r.Id == nextStep.ApproverRoleId);
                    Console.WriteLine($"Paso {stepOrder} aprobado. Siguiente paso: {nextStep.StepOrder} ({nextRole?.Name ?? "Rol desconocido"})");
                }
            }
            else
            {
                currentStep.StatusId = statusRejected.Id;
                currentStep.Status = statusRejected;
                currentStep.DecisionDate = DateTime.Now;

                projectProposal.Status = statusRejected;
                projectProposal.StatusId = statusRejected.Id;
                Console.WriteLine("El proyecto ha sido rechazado.");
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar cambios: {ex.Message}");
            }
        }


        public static void UpdateProjectStatus(ProjectApprovalDbContext context, Guid projectId, int statusId)
        {
            var projectProposal = context.ProjectProposals.FirstOrDefault(p => p.Id == projectId);
            if (projectProposal == null)
            {
                Console.WriteLine("Proyecto no encontrado.");
                return;
            }

            var status = context.ApprovalStatuses.FirstOrDefault(s => s.Id == statusId);
            if (status == null)
            {
                Console.WriteLine("Estado no válido.");
                return;
            }

            projectProposal.Status = status;
            projectProposal.StatusId = statusId;

            try
            {
                context.SaveChanges();
                Console.WriteLine($"El estado del proyecto ha sido actualizado a: {status.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el estado: {ex.Message}");
            }
        }
    }
}