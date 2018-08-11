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
        public string Title { get; set; }
        [BindProperty]
        public string Text { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            List<Note> notes = await _noteDbContext.Notes.AsNoTracking().Include(x => x.Content).Where(x => x.UserId.Equals(userId)).Where(x => x.IsPinned.Equals(true)).ToListAsync();
            notes.AddRange(await _noteDbContext.Notes.AsNoTracking().Include(x => x.Content).Where(x => x.UserId.Equals(userId)).Where(x => x.IsPinned.Equals(false)).ToListAsync());
            Notes = notes;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Title = "(Untitled note)";
            Text = "";
            Note note = new Note();
            note.Content = new NoteContent();
            note.Title = Title;
            note.Content.Text = Text;
            note.CreatedAt = DateTime.Now;
            note.IsPinned = false;
            note.IsShared = false;
            note.Url = "";
            note.UserId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            if (ModelState.IsValid == false)
            {
                return Page();
            }
            _noteDbContext.Notes.Add(note);
            await _noteDbContext.SaveChangesAsync();
            return RedirectToPage("/Dashboard");
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var note = await _noteDbContext.Notes.FindAsync(id);
            if (note != null)
            {
                _noteDbContext.Notes.Remove(note);
                //_noteDbContext.Contents.Remove(note.Content);
                await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(Guid id)
        {
            var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            //var note = await _noteDbContext.Notes.FindAsync(id);
            if (note != null)
            {
                note.Title = Title;
                note.Content.Text = Text;
                note.ModifiedAt = DateTime.Now;
                if (note.Title == null || note.Title.Equals(string.Empty) || note.Title.Length > 50)
                {
                    note.Title = "(Untitled note)";
                    Title = "(Untitled note)";
                }
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                _noteDbContext.Notes.Update(note);
                await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPinUnpinAsync(Guid id)
        {
            var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            if (note != null)
            {
                note.IsPinned = !(note.IsPinned);
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                _noteDbContext.Notes.Update(note);
                await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostShareUnshareAsync(Guid id)
        {
            var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            if (note != null)
            {
                note.IsShared = !(note.IsShared);
                if(note.IsShared == true)
                    note.Url = "https://" + Request.Host.ToString() + "/Note/" + note.Id.ToString();
                else
                    note.Url = "";
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                _noteDbContext.Notes.Update(note);
                await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}