using NoteIt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoteIt.Core.Services
{
    public interface IAzureStorageService
    {
        Task<List<Note>> GetAllNotes(Guid userId);
        Task<Note> GetNote(string id);
        Task AddNote(Note note);
        Task UpdateNote(Note note);
        Task DeleteNote(Note note);
    }
}
