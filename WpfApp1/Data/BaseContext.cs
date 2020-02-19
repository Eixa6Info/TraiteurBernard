
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Modele;

namespace TraiteurBernardWPF.Data
{

    public class BaseContext : DbContext
    {

        internal DbSet<Personne> Personnes { get; set; }
        internal DbSet<TypeTournee> TypeTournee { get; set; }
        internal DbSet<Livraison> Livraisons { get; set; }

        internal DbSet<TypeCompteDeFacturation> ComptesDeFacturation { get; set; }

        // Pour que ceci soit dispo, 
        // Install-Package Microsoft.EntityFrameworkCore.Sqlite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=traiteur.db");
    }

}
