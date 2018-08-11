using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NoteIt.Core.Entities;
using NoteIt.Infrastructure.Data;
using NoteIt.Infrastructure.Identity;

namespace NoteIt.Pages
{
    public class NoteModel : PageModel
    {
        private readonly NoteDbContext _noteDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly IConfiguration _configuration;
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly CloudTable noteTable;

        public NoteModel(NoteDbContext noteDbContext, ApplicationIdentityDbContext identityDbContext, IConfiguration configuration)
        {
            _noteDbContext = noteDbContext;
            _identityDbContext = identityDbContext;
            _configuration = configuration;
            storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnection"]);
            tableClient = new CloudTableClient(storageAccount.TableStorageUri, storageAccount.Credentials);
            noteTable = tableClient.GetTableReference("Notes");
            noteTable.CreateIfNotExistsAsync().Wait();
        }

        [BindProperty]
        public NoteEntity Note { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            //Note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            TableOperation getNote = TableOperation.Retrieve<NoteEntity>("Note", id.ToString());
            var getResult = await noteTable.ExecuteAsync(getNote);
            Note = getResult.Result as NoteEntity;
            if(Note == null)
                Note = new NoteEntity();
            await Task.CompletedTask;
        }
    }
}