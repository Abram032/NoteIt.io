using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NoteIt.Core.Entities;
using NoteIt.Infrastructure.Data;

namespace NoteIt.Pages
{
    public class NoteModel : PageModel
    {
        private readonly NoteDbContext _noteDbContext;

        public NoteModel(NoteDbContext noteDbContext)
        {
            _noteDbContext = noteDbContext;
        }

        public Note Note { get; private set; }

        public async Task OnGetAsync(Guid id)
        {
            Note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            if(Note == null)
                Note = new Note();
            await Task.CompletedTask;
        }
    }
}