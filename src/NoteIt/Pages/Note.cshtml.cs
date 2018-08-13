using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NoteIt.Core.Entities;
using NoteIt.Core.Services;
using NoteIt.Infrastructure.Identity;

namespace NoteIt.Pages
{
    public class NoteModel : PageModel
    {
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _storageService;

        public NoteModel(IAzureStorageService storageService, ApplicationIdentityDbContext identityDbContext, IConfiguration configuration)
        {
            _identityDbContext = identityDbContext;
            _configuration = configuration;
            _storageService = storageService;
        }

        [BindProperty]
        public Note Note { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            Note = await _storageService.GetNote(id.ToString());
            if(Note == null)
                Note = new Note();
            await Task.CompletedTask;
        }
    }
}