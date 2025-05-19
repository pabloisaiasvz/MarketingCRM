using MarketingCRM.Data;
using MarketingCRM.Models;
using MarketingCRM.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MarketingCRM.ConsoleUI
{
    public class ProjectApprovalManager
    {
        private readonly ProjectApprovalDbContext _context;
        private User _currentUser;

        public ProjectApprovalManager(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public void Run()
        {
            SelectUser();

            bool continuar = true;
            while (continuar)
            {
                ShowMainMenu();
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        CreateNewProject();
                        break;
                    case "2":
                        ViewMyRequests();
                        break;
                    case "3":
                        ReviewPendingRequests();
                        break;
                    case "4":
                        SelectUser();
                        break;
                    case "5":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private void SelectUser()
        {
            Console.Clear();
            Console.WriteLine("=== SELECCIÓN DE USUARIO ===");

            var usuarios = _context.Users.ToList();
            for (int i = 0; i < usuarios.Count; i++)
            {
                var rolName = _context.ApproverRoles.FirstOrDefault(r => r.Id == usuarios[i].RoleId)?.Name ?? "Sin rol";
                Console.WriteLine($"{i + 1}. {usuarios[i].Name} - {usuarios[i].Email} ({rolName})");
            }

            Console.Write("\nSeleccione un usuario (número): ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion >= 1 && seleccion <= usuarios.Count)
            {
                _currentUser = usuarios[seleccion - 1];
                Console.WriteLine($"Usuario seleccionado: {_currentUser.Name}");
            }
            else
            {
                Console.WriteLine("Selección inválida. Utilizando el primer usuario por defecto.");
                _currentUser = usuarios.First();
            }
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            var roleName = _currentUser?.RoleId.ToString() ?? "Sin rol";

            Console.Clear();
            Console.WriteLine($"=== SISTEMA DE APROBACIÓN DE PROYECTOS ===");
            Console.WriteLine($"Usuario actual: {_currentUser?.Name}");
            Console.WriteLine("\nMENU PRINCIPAL:");
            Console.WriteLine("1. Crear un proyecto nuevo");
            Console.WriteLine("2. Ver el estado de mis solicitudes de proyecto");
            Console.WriteLine("3. Revisar solicitudes de proyecto pendientes");
            Console.WriteLine("4. Cambiar de Usuario");
            Console.WriteLine("5. Salir de la aplicación");
            Console.Write("\nSeleccione una opción: ");
        }

        private void CreateNewProject()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVO PROYECTO ===");

            Console.Write("Título del Proyecto: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Descripción del Proyecto: ");
            string description = Console.ReadLine() ?? "";

            decimal estimatedAmount = 0;
            bool montoValido = false;
            while (!montoValido)
            {
                Console.Write("Monto Estimado del Proyecto (ej. 15000): ");
                montoValido = decimal.TryParse(Console.ReadLine(), out estimatedAmount);
                if (!montoValido)
                    Console.WriteLine("Por favor, ingrese un valor numérico válido.");
            }

            int duration = 0;
            bool duracionValida = false;
            while (!duracionValida)
            {
                Console.Write("Duración en Meses (ej. 6): ");
                duracionValida = int.TryParse(Console.ReadLine(), out duration);
                if (!duracionValida)
                    Console.WriteLine("Por favor, ingrese un valor numérico válido.");
            }

            Console.WriteLine("\nÁreas disponibles:");
            var areas = _context.Areas.ToList();
            foreach (var area in areas)
            {
                Console.WriteLine($"{area.Id}. {area.Name}");
            }

            int areaId = 0;
            bool areaValida = false;
            while (!areaValida)
            {
                Console.Write("Seleccione el ID del Área: ");
                areaValida = int.TryParse(Console.ReadLine(), out areaId);
                areaValida = areaValida && areas.Any(a => a.Id == areaId);
                if (!areaValida)
                    Console.WriteLine("Por favor, seleccione un ID de área válido.");
            }

            Console.WriteLine("\nTipos de Proyecto disponibles:");
            var tipos = _context.ProjectTypes.ToList();
            foreach (var tipo in tipos)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Name}");
            }

            int projectTypeId = 0;
            bool tipoValido = false;
            while (!tipoValido)
            {
                Console.Write("Seleccione el ID del Tipo de Proyecto: ");
                tipoValido = int.TryParse(Console.ReadLine(), out projectTypeId);
                tipoValido = tipoValido && tipos.Any(t => t.Id == projectTypeId);
                if (!tipoValido)
                    Console.WriteLine("Por favor, seleccione un ID de tipo válido.");
            }

            ProjectApprovalUtils.CreateProjectProposal(_context, title, description, estimatedAmount, duration, areaId, projectTypeId, _currentUser.Id);
        }

        private void ViewMyRequests()
        {
            Console.Clear();
            Console.WriteLine("=== MIS SOLICITUDES DE PROYECTO ===");

            var proyectos = _context.ProjectProposals
                .Where(p => p.CreateById == _currentUser.Id)
                .Include(p => p.Area)
                .Include(p => p.Type)
                .ToList();

            if (!proyectos.Any())
            {
                Console.WriteLine("No tienes solicitudes de proyecto registradas.");
                return;
            }

            Console.WriteLine("\nListado de Proyectos:");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("| # | Título | Monto | Área | Tipo | Estado |");
            Console.WriteLine("-----------------------------------------------------------------");

            for (int i = 0; i < proyectos.Count; i++)
            {
                var proyecto = proyectos[i];

                var steps = _context.ProjectApprovalSteps
                    .Include(s => s.Status)
                    .Where(s => s.ProjectProposalId == proyecto.Id)
                    .ToList();

                string estado = "Pending";
                if (steps.Any(s => s.Status.Name == "Rejected"))
                    estado = "Rejected";
                else if (steps.All(s => s.Status.Name == "Approved"))
                    estado = "Approved";

                var areaName = proyecto.Area?.Name ?? "N/A";
                var typeName = proyecto.Type?.Name ?? "N/A";

                Console.WriteLine($"| {i + 1} | {proyecto.Title.PadRight(15).Substring(0, 15)} | {proyecto.EstimatedAmount.ToString("C")} | {areaName} | {typeName} | {estado} |");
            }

            Console.WriteLine("-----------------------------------------------------------------");

            Console.Write("\n¿Desea ver detalles de algún proyecto? (S/N): ");
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                Console.Write("Ingrese el número del proyecto: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= proyectos.Count)
                {
                    var proyectoSeleccionado = proyectos[index - 1];
                    ShowProjectDetails(proyectoSeleccionado.Id);
                }
                else
                {
                    Console.WriteLine("Número de proyecto no válido.");
                }
            }
        }

        private void ShowProjectDetails(Guid projectId)
        {
            var proyecto = _context.ProjectProposals
                .Include(p => p.Status)
                .Include(p => p.Area)
                .Include(p => p.Type)
                .FirstOrDefault(p => p.Id == projectId);

            if (proyecto == null)
            {
                Console.WriteLine("Proyecto no encontrado.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== DETALLES DEL PROYECTO: {proyecto.Title} ===");
            Console.WriteLine($"ID: {proyecto.Id}");
            Console.WriteLine($"Descripción: {proyecto.Description}");
            Console.WriteLine($"Monto Estimado: {proyecto.EstimatedAmount:C}");
            Console.WriteLine($"Duración Estimada: {proyecto.EstimatedDuration} meses");
            Console.WriteLine($"Área: {proyecto.Area?.Name}");
            Console.WriteLine($"Tipo: {proyecto.Type?.Name}");
            Console.WriteLine($"Estado: {proyecto.Status?.Name}");
            Console.WriteLine($"Fecha de Creación: {proyecto.CreateAt}");

            Console.WriteLine("\nPasos de Aprobación:");
            var pasos = _context.ProjectApprovalSteps
                .Include(s => s.ApproverRole)
                .Include(s => s.Status)
                .Where(s => s.ProjectProposalId == projectId)
                .OrderBy(s => s.StepOrder)
                .ToList();

            if (!pasos.Any())
            {
                Console.WriteLine("No hay pasos de aprobación definidos para este proyecto.");
                return;
            }

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("| Paso | Rol Aprobador | Estado | Fecha Aprobación |");
            Console.WriteLine("------------------------------------------------------------");

            foreach (var paso in pasos)
            {
                string approverRoleName = paso.ApproverRole?.Name ?? "N/A";
                string statusName = paso.Status?.Name ?? "N/A";
                string approvedDate = paso.DecisionDate.HasValue ? paso.DecisionDate.Value.ToString("dd/MM/yyyy") : "Pendiente";

                Console.WriteLine($"| {paso.StepOrder} | {approverRoleName.PadRight(15).Substring(0, 15)} | {statusName.PadRight(10).Substring(0, 10)} | {approvedDate} |");
            }
            Console.WriteLine("------------------------------------------------------------");
        }

        private void ReviewPendingRequests()
        {
            Console.Clear();
            Console.WriteLine("=== REVISAR SOLICITUDES PENDIENTES ===");

            var pendingSteps = _context.ProjectApprovalSteps
                .Include(s => s.ProjectProposal)
                .Include(s => s.Status)
                .Include(s => s.ApproverRole)
                .Where(s => s.ApproverRoleId == _currentUser.RoleId && s.Status.Name == "Pending")
                .OrderBy(s => s.ProjectProposal.CreateAt)
                .ToList();

            if (!pendingSteps.Any())
            {
                Console.WriteLine($"No hay solicitudes pendientes para su aprobación como {_context.ApproverRoles.FirstOrDefault(r => r.Id == _currentUser.RoleId)?.Name}.");
                return;
            }

            Console.WriteLine("\nSolicitudes pendientes para su aprobación:");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("| # | ID Proyecto | Título | Monto | Paso | Rol Aprobador |");
            Console.WriteLine("------------------------------------------------------------------");

            for (int i = 0; i < pendingSteps.Count; i++)
            {
                var step = pendingSteps[i];
                string projectId = step.ProjectProposalId.ToString().Substring(0, 8) + "...";
                string title = step.ProjectProposal?.Title?.PadRight(15).Substring(0, 15) ?? "N/A";
                string amount = step.ProjectProposal?.EstimatedAmount.ToString("C") ?? "N/A"; 
                string approverRole = step.ApproverRole?.Name ?? "N/A";

                Console.WriteLine($"| {i + 1} | {projectId} | {title} | {amount} | {step.StepOrder} | {approverRole} |");
            }
            Console.WriteLine("------------------------------------------------------------------");

            Console.Write("\nSeleccione el número de la solicitud a revisar (0 para salir): ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= pendingSteps.Count)
            {
                var selectedStep = pendingSteps[seleccion - 1];
                ReviewRequest(selectedStep, _currentUser.Id);
            }
            else if (seleccion != 0)
            {
                Console.WriteLine("Selección no válida.");
            }
        }


        private void ReviewRequest(ProjectApprovalStep step, int userId)
        {
            var projectId = step.ProjectProposalId;
            ShowProjectDetails(projectId);

            Console.WriteLine("\n=== REVISAR SOLICITUD ===");
            Console.WriteLine($"Está revisando el paso {step.StepOrder} como {step.ApproverRole?.Name}");
            Console.WriteLine("1. Aprobar");
            Console.WriteLine("2. Rechazar");
            Console.WriteLine("3. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ProjectApprovalUtils.ProcessApproval(_context, projectId, true, step.StepOrder, userId);
                    Console.WriteLine("Solicitud aprobada correctamente.");
                    break;
                case "2":
                    ProjectApprovalUtils.ProcessApproval(_context, projectId, false, step.StepOrder, userId);
                    Console.WriteLine("Solicitud rechazada.");
                    break;
                case "3":
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }
}