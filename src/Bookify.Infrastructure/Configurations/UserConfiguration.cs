using Bookify.Domain.Apartments;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(a => a.FirstName)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new FirstName(value));

        builder.Property(a => a.LastName)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new LastName(value));

        builder.Property(a => a.Email)
            .HasMaxLength(200)
            .HasConversion(email => email.Value, value => new Domain.Users.Email(value));

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(user => user.IdentityId).IsUnique();
    }
}