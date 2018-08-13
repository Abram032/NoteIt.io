using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NoteIt.Core.Entities;
using NoteIt.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteIt.Infrastructure.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private IConfiguration Configuration { get; set; }
        private CloudTable NoteTable { get; set; }

        public AzureStorageService(IConfiguration configuration)
        {
            Configuration = configuration;
            NoteTable = GetTable();
        }

        private CloudTable GetTable()
        {
            var storageAccount = CloudStorageAccount.Parse(Configuration["StorageConnection"]);
            var tableClient = new CloudTableClient(storageAccount.TableStorageUri, storageAccount.Credentials);
            var noteTable = tableClient.GetTableReference("Notes");
            noteTable.CreateIfNotExistsAsync().Wait();
            return noteTable;
        }

        public async Task AddNote(Note note)
        {
            TableOperation operation = TableOperation.Insert(note);
            await NoteTable.ExecuteAsync(operation);
        }

        public async Task DeleteNote(Note note)
        {
            TableOperation deleteNote = TableOperation.Delete(note);
            await NoteTable.ExecuteAsync(deleteNote);
        }

        public async Task<List<Note>> GetAllNotes(Guid userId)
        {
            List<Note> notes = new List<Note>();
            TableQuery<Note> query = new TableQuery<Note>().Where(TableQuery.GenerateFilterConditionForGuid("UserId", QueryComparisons.Equal, userId));
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResult = await NoteTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                notes.AddRange(queryResult.ToList());
                continuationToken = queryResult.ContinuationToken;

            } while(continuationToken != null);
            List<Note> pinnedNotes = notes.Where(x => x.IsPinned.Equals(true)).ToList();
            List<Note> unpinnedNotes = notes.Where(x => x.IsPinned.Equals(false)).ToList();
            List<Note> allNotes = new List<Note>();
            allNotes.AddRange(pinnedNotes);
            allNotes.AddRange(unpinnedNotes);
            return allNotes;
        }

        public async Task<Note> GetNote(string id)
        {
            TableOperation getNote = TableOperation.Retrieve<Note>("Note", id);
            var getResult = await NoteTable.ExecuteAsync(getNote);
            var note = getResult.Result as Note;
            return note;
        }

        public async Task UpdateNote(Note note)
        {
            TableOperation updateNote = TableOperation.Replace(note);
            await NoteTable.ExecuteAsync(updateNote);
        }
    }
}
