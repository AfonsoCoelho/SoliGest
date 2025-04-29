using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos painéis solares.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SolarPanelsController : Controller
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador de painéis solares.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public SolarPanelsController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os painéis solares.
        /// </summary>
        /// <returns>Lista de objetos <see cref="SolarPanel"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolarPanel>>> GetSolarPanel()
        {
            return await _context.SolarPanel.ToListAsync();
        }

        /// <summary>
        /// Cria um novo painel solar.
        /// </summary>
        /// <param name="solarPanel">Objeto <see cref="SolarPanel"/> a ser criado.</param>
        /// <returns>Resposta HTTP com o resultado da operação.</returns>
        [HttpPost]
        public async Task<IActionResult> PostSolarPanel(SolarPanel solarPanel)
        {
            try
            {
                _context.SolarPanel.Add(solarPanel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSolarPanel", new { id = solarPanel.Id }, solarPanel);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove um painel solar pelo ID.
        /// </summary>
        /// <param name="id">ID do painel solar a remover.</param>
        /// <returns>Resposta HTTP com o resultado da operação.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolarPanel(int id)
        {
            try
            {
                var solarPanel = await _context.FindAsync<SolarPanel>(id);
                if (solarPanel == null)
                {
                    return NotFound($"Não foi possível encontrar o painel solar com o ID '{id}'.");
                }

                _context.Remove<SolarPanel>(solarPanel);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtém um painel solar pelo ID.
        /// </summary>
        /// <param name="id">ID do painel solar.</param>
        /// <returns>Objeto <see cref="SolarPanel"/> correspondente, se existir.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SolarPanel>> GetSolarPanel(int id)
        {
            try
            {
                var solarPanel = await _context.FindAsync<SolarPanel>(id);
                if (solarPanel != null)
                {
                    return solarPanel;
                }
                else
                {
                    return NotFound($"Não foi possível encontrar o painel solar com o ID '{id}'.");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualiza os dados de um painel solar existente.
        /// </summary>
        /// <param name="model">Modelo com os dados atualizados do painel solar.</param>
        /// <returns>Resposta HTTP com o resultado da operação.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolarPanel([FromBody] SolarPanelUpdateModel model)
        {
            var solarPanel = await _context.FindAsync<SolarPanel>(model.Id);
            if (solarPanel == null)
            {
                return NotFound($"Não foi possível encontrar o painel solar com o ID '{model.Id}'.");
            }

            solarPanel.Name = model.Name;
            solarPanel.Priority = model.Priority;
            solarPanel.Status = model.Status;
            solarPanel.StatusClass = model.StatusClass;
            solarPanel.Latitude = model.Latitude;
            solarPanel.Longitude = model.Longitude;
            solarPanel.Description = model.Description;
            solarPanel.PhoneNumber = model.PhoneNumber;
            solarPanel.Email = model.Email;
            solarPanel.Address = model.Address;

            _context.Update<SolarPanel>(solarPanel);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Painel solar atualizado com sucesso!" });
        }
    }

    /// <summary>
    /// Modelo de dados para atualização de um painel solar.
    /// </summary>
    public class SolarPanelUpdateModel
    {
        /// <summary>
        /// ID do painel solar.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do painel solar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prioridade associada ao painel.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Estado atual do painel solar.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Classe CSS de status (para visualização).
        /// </summary>
        public string StatusClass { get; set; }

        /// <summary>
        /// Latitude do painel solar.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude do painel solar.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Descrição do painel solar.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Número de telefone associado ao painel.
        /// </summary>
        public int PhoneNumber { get; set; }

        /// <summary>
        /// Email de contacto.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Endereço físico do painel solar.
        /// </summary>
        public string Address { get; set; }
    }
}
