using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Contexto do banco de dados da aplicação SoliGest, herda de IdentityDbContext para a gestão de usuários.
    /// Define as entidades que o Entity Framework irá mapear para o banco de dados, incluindo Painéis Solares, Solicitações de Assistência, Conversas, Mensagens, e Notificações.
    /// </summary>
    public class SoliGestServerContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Construtor que inicializa o contexto com as opções de configuração.
        /// </summary>
        /// <param name="options">Opções de configuração para o contexto do banco de dados.</param>
        public SoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tabela de painéis solares.
        /// </summary>
        public DbSet<SolarPanel> SolarPanel { get; set; } = default!;

        /// <summary>
        /// Tabela de endereços.
        /// </summary>
        public DbSet<Address> Address { get; set; } = default!;

        /// <summary>
        /// Tabela de solicitações de assistência.
        /// </summary>
        public DbSet<AssistanceRequest> AssistanceRequest { get; set; } = default!;

        /// <summary>
        /// Tabela de dias de folga.
        /// </summary>
        public DbSet<DayOff> DaysOff { get; set; }

        /// <summary>
        /// Tabela de feriados.
        /// </summary>
        public DbSet<Holidays> Holidays { get; set; }

        /// <summary>
        /// Tabela de conversas.
        /// </summary>
        public DbSet<Conversation> Conversations { get; set; }

        /// <summary>
        /// Tabela de mensagens.
        /// </summary>
        public DbSet<Message> Messages { get; set; }

        /// <summary>
        /// Tabela de notificações.
        /// </summary>
        public DbSet<Notification> Notification { get; set; }

        /// <summary>
        /// Tabela de notificações de usuários.
        /// </summary>
        public DbSet<UserNotification> UserNotification { get; set; }

        /// <summary>
        /// Configuração das relações entre as entidades utilizando o Fluent API.
        /// </summary>
        /// <param name="modelBuilder">Modelo de construção de entidades.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento Many-to-Many entre Conversation e User
            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Conversations)
                .UsingEntity<Dictionary<string, object>>(
                    "ConversationUser",
                    join => join
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join
                        .HasOne<Conversation>()
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ConversationId", "UserId");
                        join.ToTable("ConversationUsers");
                    }
                );

            // Relacionamento 1:N entre Conversation e Contact
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Contact)
                .WithMany()
                .HasForeignKey("ContactId")
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Message e Sender (User)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Message e Receiver (User)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento N:1 entre Message e Conversation
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
