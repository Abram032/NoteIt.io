using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NoteIt.Core.Entities;
using NoteIt.Infrastructure.Data;
using NoteIt.Infrastructure.Identity;

namespace NoteIt.Pages
{
    [Authorize(Policy = "UserOrAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly NoteDbContext _noteDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;

        public DashboardModel(NoteDbContext noteDbContext, ApplicationIdentityDbContext identityDbContext)
        {
            _noteDbContext = noteDbContext;
            _identityDbContext = identityDbContext;
        }

        public IList<Note> Notes { get; private set; }

        [BindProperty]
        public Note Note { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            Notes = await _noteDbContext.Notes.AsNoTracking().Include(x => x.Content).Where(x => x.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Note.CreatedAt = DateTime.Now;
            Note.IsPinned = false;
            Note.UserId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            if(Note.Title == null || Note.Title.Equals(string.Empty))
                Note.Title = "(Untitled note)";
            if(ModelState.IsValid == false)
            {
                return Page();
            }
            _noteDbContext.Notes.Add(Note);
            await _noteDbContext.SaveChangesAsync();
            return RedirectToPage("/Dashboard");
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var note = await _noteDbContext.Notes.FindAsync(id);
            if(note != null)
            {
                _noteDbContext.Notes.Remove(note);
                //_noteDbContext.Contents.Remove(note.Content);
                await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}