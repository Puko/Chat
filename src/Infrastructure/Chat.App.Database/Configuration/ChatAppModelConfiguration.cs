using Chat.App.Database.Entities;
using Chat.App.Database.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Configuration;

public static class ChatAppModelConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Username).IsUnique();
            entity.Property(user => user.Username).HasMaxLength(StringLengths.UserName);
            entity.Property(user => user.PasswordHash).HasMaxLength(StringLengths.UserPasswordHash);
            entity.Property(user => user.Role)
                .HasConversion<string>()
                .HasMaxLength(StringLengths.UserRole);
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversations");
            entity.HasKey(conversation => conversation.Id);
            entity.Property(conversation => conversation.Type)
                .HasConversion<string>()
                .HasMaxLength(StringLengths.ConversationType);
            entity.HasIndex(conversation => conversation.LastMessageAtUtc);
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.ToTable("ConversationParticipants");
            entity.HasKey(participant => new { participant.ConversationId, participant.UserId });

            entity.HasOne(participant => participant.Conversation)
                .WithMany(conversation => conversation.Participants)
                .HasForeignKey(participant => participant.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(participant => participant.User)
                .WithMany()
                .HasForeignKey(participant => participant.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(participant => participant.UserId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Content).HasMaxLength(StringLengths.MessageContent);

            entity.HasOne(message => message.Conversation)
                .WithMany(conversation => conversation.Messages)
                .HasForeignKey(message => message.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(message => message.Sender)
                .WithMany()
                .HasForeignKey(message => message.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(message => new { message.ConversationId, message.SentAtUtc });
            entity.HasIndex(message => message.SenderId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(refreshToken => refreshToken.Id);
            entity.Property(refreshToken => refreshToken.TokenHash).HasMaxLength(StringLengths.RefreshTokenHash);
            entity.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();
            entity.HasIndex(refreshToken => refreshToken.UserId);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(refreshToken => refreshToken.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
