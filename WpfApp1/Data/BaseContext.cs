
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.Modele;
using Microsoft.EntityFrameworkCore;

namespace TraiteurBernardWPF.Data
{

    public class BaseContext : DbContext
    {

        public DbSet<Personne> Personnes { get; set; }
        public DbSet<TypeTournee> TypeTournee { get; set; }
        public DbSet<Livraison> Livraisons { get; set; }
        public DbSet<TypeCompteDeFacturation> ComptesDeFacturation { get; set; }
        public DbSet<ContactDurgence> ContactDurgence { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Plat> Plat { get; set; }

        public DbSet<Saisie> Saisies { get; set; }
        public DbSet<SaisieData> SaisieData { get; set; }

        // Pour que ceci soit dispo, 
        // Install-Package Microsoft.EntityFrameworkCore.Sqlite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
                                                                                            .UseSqlite("Data Source=traiteur.db")
                                                                                            .EnableSensitiveDataLogging(true);
    }

}
