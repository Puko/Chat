using Chat.App.Database.Abstraction;
using Chat.App.Database.Abstraction.Abstracts;
using Chat.App.Database.Configuration;
using Chat.App.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chat.App.Database.Contexts;

public sealed class ChatAppDbContext(DbContextOptions<ChatAppDbContext> options)
    : DbContext(options), IDbContextAccessor, IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct) => SaveChangesAsync(ct);

    Task<IDbContextTransaction> IUnitOfWork.BeginTransactionAsync(CancellationToken ct)
        => Database.BeginTransactionAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ChatAppModelConfiguration.Configure(modelBuilder);
    }
}
