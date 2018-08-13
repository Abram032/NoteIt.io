using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NoteIt.Core.Entities;
using NoteIt.Core.Services;
using NoteIt.Infrastructure.Identity;
using NoteIt.ViewModels;

namespace NoteIt.Pages
{
    [Authorize(Policy = "UserOrAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _storageService;

        public DashboardModel(ApplicationIdentityDbContext identityDbContext, IConfiguration configuration, IAzureStorageService storageService)
        {
            _identityDbContext = identityDbContext;
            _configuration = configuration;
            _storageService = storageService;
        }

        public IList<Note> Notes { get; private set; }

        [BindProperty]
        public DashboardViewModel Note { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            Notes = await _storageService.GetAllNotes(userId);
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            Note note = new Note("Note", Guid.NewGuid())
            {
                Title = Note.Title,
                Content = Note.Content,
                IsPinned = Note.IsPinned,
                IsShared = Note.IsShared,
                UserId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault()
            };

            await _storageService.AddNote(note);

            return RedirectToPage("/Dashboard");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var note = await _storageService.GetNote(id);
            if (note != null)
            {
                await _storageService.DeleteNote(note);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(string id)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var note = await _storageService.GetNote(id);
            if (note != null)
            {
                note.Title = Note.Title;
                note.Content = Note.Content;
                note.ModifiedAt = DateTime.Now;
                await _storageService.UpdateNote(note);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPinUnpinAsync(string id)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var note = await _storageService.GetNote(id);
            if (note != null)
            {
                note.IsPinned = !(note.IsPinned);
                await _storageService.UpdateNote(note);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostShareUnshareAsync(string id)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var note = await _storageService.GetNote(id);
            if (note != null)
            {
                note.IsShared = !(note.IsShared);
                note.Url = "https://" + Request.Host.ToString() + "/Note/" + note.RowKey.ToString();
                await _storageService.UpdateNote(note);
            }
            return RedirectToPage();
        }
    }
}