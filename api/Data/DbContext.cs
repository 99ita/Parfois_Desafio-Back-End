using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Item> Itens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>()
                .ToTable("Pedidos")
                .HasKey(p => p.PedidoId);

            modelBuilder.Entity<Item>()
                .ToTable("Itens")
                .HasKey(i => i.Id);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Pedido) 
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.PedidoId);
        }
    }
}