﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SoliGestTest
{
    /// <summary>
    /// Testes unitários para o controller <see cref="MetricsController"/>.
    /// </summary>
    public class MetricsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly MetricsController _controller;

        /// <summary>
        /// Inicializa o ambiente de testes, criando uma base de dados isolada em memória.
        /// </summary>
        public MetricsControllerTest()
        {
            var dbName = Guid.NewGuid().ToString();
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new SoliGestServerContext(_options);
            _controller = new MetricsController(_context);
        }

        /// <summary>
        /// Cria um painel solar fictício com as propriedades necessárias.
        /// </summary>
        private SolarPanel CreateDummyPanel() => new SolarPanel
        {
            Name = "panel",
            Description = "desc",
            Address = "addr",
            Email = "email@example.com",
            Status = "ok",
            StatusClass = "info",
            Priority = "low"
        };

        /// <summary>
        /// Cria um usuário fictício com as propriedades necessárias.
        /// </summary>
        private User CreateDummyUser() => new User
        {
            Name = "User Test",
            Address1 = "Address 1",
            Address2 = "Address 2",
            BirthDate = "1990-01-01",
            DayOff = "Monday",
            StartHoliday = "2025-01-01",
            EndHoliday = "2025-01-15",
            Role = "Employee"
        };

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetTotalUsers"/> retorna zero quando não há usuários.
        /// </summary>
        [Fact]
        public async Task GetTotalUsers_ReturnsZero_WhenNoUsers()
        {
            var result = await _controller.GetTotalUsers();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(0, jsonObj["totalUsers"].Value<int>());
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetTotalUsers"/> retorna a contagem correta de usuários quando existem usuários na base de dados.
        /// </summary>
        [Fact]
        public async Task GetTotalUsers_ReturnsCount_WhenUsersExist()
        {
            _context.Users.AddRange(
                CreateDummyUser(),
                CreateDummyUser(),
                CreateDummyUser()
            );
            await _context.SaveChangesAsync();

            var result = await _controller.GetTotalUsers();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(3, jsonObj["totalUsers"].Value<int>());
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetTotalPainels"/> retorna a contagem correta de painéis solares.
        /// </summary>
        [Fact]
        public async Task GetTotalPaineis_ReturnsCount()
        {
            _context.SolarPanel.AddRange(CreateDummyPanel(), CreateDummyPanel());
            await _context.SaveChangesAsync();

            var result = await _controller.GetTotalPainels();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(2, jsonObj["totalPanels"].Value<int>());
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetTotalAssistanceRequests"/> retorna a contagem correta de solicitações de assistência.
        /// </summary>
        [Fact]
        public async Task GetTotalAssistanceRequests_ReturnsCount()
        {
            var panel = CreateDummyPanel();
            _context.SolarPanel.Add(panel);
            _context.AssistanceRequest.AddRange(
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", RequestDate = "", ResolutionDate = "", Priority = "", SolarPanel = panel },
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", RequestDate = "", ResolutionDate = "", Priority = "", SolarPanel = panel },
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", RequestDate = "", ResolutionDate = "", Priority = "", SolarPanel = panel }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.GetTotalAssistanceRequests();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(3, jsonObj["totalAssistanceRequests"].Value<int>());
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetAssistanceRequestPerStatus"/> retorna a contagem agrupada das solicitações de assistência por status.
        /// </summary>
        [Fact]
        public async Task GetAssistanceRequestPerStatus_ReturnsGroupedCounts()
        {
            var panel = CreateDummyPanel();
            _context.SolarPanel.Add(panel);
            _context.AssistanceRequest.AddRange(
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", Priority = "High", RequestDate = "", ResolutionDate = "", SolarPanel = panel },
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", Priority = "High", RequestDate = "", ResolutionDate = "", SolarPanel = panel },
                new AssistanceRequest { Description = "desc", Status = "open", StatusClass = "info", Priority = "Low", RequestDate = "", ResolutionDate = "", SolarPanel = panel }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.GetAssistanceRequestPerStatus();
            var ok = Assert.IsType<OkObjectResult>(result);
            var dict = Assert.IsType<Dictionary<string, int>>(ok.Value);
            Assert.Equal(2, dict["High"]);
            Assert.Equal(1, dict["Low"]);
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetAverageRepairTime"/> retorna zero quando não há solicitações resolvidas.
        /// </summary>
        [Fact]
        public async Task GetAverageRepairTime_ReturnsZero_WhenNoResolvedRequests()
        {
            var panel = CreateDummyPanel();
            _context.SolarPanel.Add(panel);
            _context.AssistanceRequest.Add(new AssistanceRequest
            {
                Description = "desc",
                Status = "open",
                StatusClass = "info",
                RequestDate = "2025-04-01T08:00:00",
                ResolutionDate = "", // String vazia em vez de null
                Priority = "",
                SolarPanel = panel
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetAverageRepairTime();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(0.0, jsonObj["averageRepairTime"].Value<double>());
        }

        /// <summary>
        /// Verifica se o método <see cref="MetricsController.GetAverageRepairTime"/> retorna a média correta de tempo de reparo quando há solicitações resolvidas.
        /// </summary>
        [Fact]
        public async Task GetAverageRepairTime_ReturnsCorrectAverage_WhenResolvedRequestsExist()
        {
            var panel = CreateDummyPanel();
            _context.SolarPanel.Add(panel);
            _context.AssistanceRequest.AddRange(
                new AssistanceRequest
                {
                    Description = "desc",
                    Status = "closed",
                    StatusClass = "success",
                    RequestDate = "2025-04-01T08:00:00",
                    ResolutionDate = "2025-04-01T10:00:00",
                    Priority = "",
                    SolarPanel = panel
                },
                new AssistanceRequest
                {
                    Description = "desc",
                    Status = "closed",
                    StatusClass = "success",
                    RequestDate = "2025-04-01T09:00:00",
                    ResolutionDate = "2025-04-01T09:30:00",
                    Priority = "",
                    SolarPanel = panel
                }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.GetAverageRepairTime();
            var ok = Assert.IsType<OkObjectResult>(result);

            var jsonResult = JsonConvert.SerializeObject(ok.Value);
            var jsonObj = JObject.Parse(jsonResult);

            Assert.Equal(75.0, jsonObj["averageRepairTime"].Value<double>());
        }
    }
}
