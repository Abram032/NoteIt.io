using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteIt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteIt.Infrastructure.Data.Configuration
{
    public class NoteContentConfiguration : BaseEntityConfiguration<NoteContent>
    {
        public override void Configure(EntityTypeBuilder<NoteContent> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Note).WithOne(x => x.Content).HasForeignKey<NoteContent>(x => x.Id).IsRequired();
        }
    }
}
