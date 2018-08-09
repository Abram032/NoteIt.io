using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteIt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteIt.Infrastructure.Data.Configuration
{
    public class NoteConfiguration : BaseEntityConfiguration<Note>
    {
        public override void Configure(EntityTypeBuilder<Note> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsPinned).IsRequired();

            builder.HasOne(x => x.Content).WithOne(x => x.Note).HasForeignKey<NoteContent>(x => x.NoteId).IsRequired();
        }
    }
}
