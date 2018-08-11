using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Policy = "UserOrAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly NoteDbContext _noteDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly IConfiguration _configuration;
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly CloudTable noteTable;

        public DashboardModel(NoteDbContext noteDbContext, ApplicationIdentityDbContext identityDbContext, IConfiguration configuration)
        {
            _noteDbContext = noteDbContext;
            _identityDbContext = identityDbContext;
            _configuration = configuration;
            storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnection"]);
            tableClient = new CloudTableClient(storageAccount.TableStorageUri, storageAccount.Credentials);
            noteTable = tableClient.GetTableReference("Notes");
            noteTable.CreateIfNotExistsAsync().Wait();
        }

        public IList<NoteEntity> Notes { get; private set; }

        [BindProperty]
        public NoteEntity Note { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();
            //List<NoteEntity> notes = await _noteDbContext.Notes.AsNoTracking().Include(x => x.Content).Where(x => x.UserId.Equals(userId)).Where(x => x.IsPinned.Equals(true)).ToListAsync();
            //notes.AddRange(await _noteDbContext.Notes.AsNoTracking().Include(x => x.Content).Where(x => x.UserId.Equals(userId)).Where(x => x.IsPinned.Equals(false)).ToListAsync());
            //Notes = notes;
            List<NoteEntity> _notes = new List<NoteEntity>();
            TableQuery<NoteEntity> query = new TableQuery<NoteEntity>().Where(TableQuery.GenerateFilterConditionForGuid("UserId", QueryComparisons.Equal, userId));
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResult = await noteTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                _notes.AddRange(queryResult.ToList());
                continuationToken = queryResult.ContinuationToken;

            } while(continuationToken != null);
            List<NoteEntity> pinnedNotes = _notes.Where(x => x.IsPinned.Equals(true)).ToList();
            List<NoteEntity> unpinnedNotes = _notes.Where(x => x.IsPinned.Equals(false)).ToList();
            List<NoteEntity> allNotes = new List<NoteEntity>();
            allNotes.AddRange(pinnedNotes);
            allNotes.AddRange(unpinnedNotes);
            Notes = allNotes;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Guid guid = Guid.NewGuid();
            string region = "Note";
            NoteEntity note = new NoteEntity(region, guid);
            note.Title = "(Untitled note)";
            note.Content = "";
            note.CreatedAt = DateTime.Now;
            note.ModifiedAt = null;
            note.IsPinned = false;
            note.IsShared = false;
            note.Url = "";
            note.UserId = _identityDbContext.Users.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.Id).SingleOrDefault();

            if (ModelState.IsValid == false)
            {
                return Page();
            }
            //_noteDbContext.Notes.Add(note);
            //await _noteDbContext.SaveChangesAsync();

            TableOperation operation = TableOperation.Insert(note);
            //await noteTable.ExecuteAsync(operation);
            await noteTable.ExecuteAsync(operation);
            return RedirectToPage("/Dashboard");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            //var note = await _noteDbContext.Notes.FindAsync(id);
            TableOperation getNote = TableOperation.Retrieve<NoteEntity>("Note", id);
            var getResult = await noteTable.ExecuteAsync(getNote);
            var note = getResult.Result as NoteEntity;
            if (note != null)
            {
                TableOperation deleteNote = TableOperation.Delete(note);
                await noteTable.ExecuteAsync(deleteNote);
                //_noteDbContext.Contents.Remove(note.Content);
                //await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(string id)
        {
            //var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            //var note = await _noteDbContext.Notes.FindAsync(id);
            TableOperation getNote = TableOperation.Retrieve<NoteEntity>("Note", id);
            var getResult = await noteTable.ExecuteAsync(getNote);
            var note = getResult.Result as NoteEntity;
            if (note != null)
            {
                note.Title = Note.Title;
                note.Content = Note.Content;
                note.ModifiedAt = DateTime.Now;
                if (note.Title == null || note.Title.Equals(string.Empty) || note.Title.Length > 50)
                {
                    note.Title = "(Untitled note)";
                    Note.Title = "(Untitled note)";
                }
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                //_noteDbContext.Notes.Update(note);
                TableOperation updateNote = TableOperation.Replace(note);
                await noteTable.ExecuteAsync(updateNote);
                //await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPinUnpinAsync(string id)
        {
            //var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            TableOperation getNote = TableOperation.Retrieve<NoteEntity>("Note", id);
            var getResult = await noteTable.ExecuteAsync(getNote);
            var note = getResult.Result as NoteEntity;
            if (note != null)
            {
                note.IsPinned = !(note.IsPinned);
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                TableOperation updateNote = TableOperation.Replace(note);
                await noteTable.ExecuteAsync(updateNote);
                //_noteDbContext.Notes.Update(note);
                //await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostShareUnshareAsync(string id)
        {
            //var note = _noteDbContext.Notes.Include(x => x.Content).Where(x => x.Id.Equals(id)).SingleOrDefault();
            TableOperation getNote = TableOperation.Retrieve<NoteEntity>("Note", id);
            var getResult = await noteTable.ExecuteAsync(getNote);
            var note = getResult.Result as NoteEntity;
            if (note != null)
            {
                note.IsShared = !(note.IsShared);
                if (note.IsShared == true)
                    note.Url = "https://" + Request.Host.ToString() + "/Note/" + note.RowKey.ToString();
                else
                    note.Url = "";
                if (ModelState.IsValid == false)
                {
                    return Page();
                }
                TableOperation updateNote = TableOperation.Replace(note);
                await noteTable.ExecuteAsync(updateNote);
                //_noteDbContext.Notes.Update(note);
                //await _noteDbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

    public class NoteEntity : TableEntity
    {
        public NoteEntity()
        {
        }

        public NoteEntity(string region, Guid guid)
        {
            this.PartitionKey = region;
            this.RowKey = guid.ToString();
        }

        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool IsShared { get; set; }
        public string Url { get; set; }
    }
}