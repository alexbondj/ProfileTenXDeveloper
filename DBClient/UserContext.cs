using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBClient.Entities;

namespace DBClient
{
	public class UserContext : DbContext
	{
		public UserContext() : base("DBConnection") {
		}

		public DbSet<Changeset> Changesets { get; set; }
		public DbSet<CodeReview> CodeReviews { get; set; }
		public DbSet<ChangesetFile> ChangesetFiles { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Properties<DateTime>()
				.Configure(c => c.HasColumnType("datetime2"));

			modelBuilder.Entity<Changeset>()
				.HasMany(cs => cs.ChangesetFiles)
				.WithRequired(cs => cs.Changeset);
			modelBuilder.Entity<Changeset>()
				.HasMany(cs => cs.CodeReviews)
				.WithRequired(cs => cs.Changeset);
			base.OnModelCreating(modelBuilder);
		}
	}

	public class InitConrtext : DropCreateDatabaseAlways<UserContext>
	{
		public override void InitializeDatabase(UserContext context)
		{
			base.InitializeDatabase(context);
		}
	}
}
