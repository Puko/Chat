using Chat.App.Contract.Chat;
using Chat.App.Contract.Paging;
using Chat.App.Contract.Users;
using Chat.App.Wpf.Services.Http;
using Chat.App.Wpf.Services.Http.ErrorMessages;
using ChatApp.Http;

namespace Chat.App.Wpf.Services;

public sealed class ConversationsService(
    JsonHttpClient httpClient,
    TokenStorageService tokenStorage,
    HttpErrorHandler errorHandler)
    : BaseApiService(httpClient, tokenStorage, errorHandler)
{
    protected override IReadOnlyList<ErrorCodeMessageResolver> ErrorResolvers { get; } =
    [
        ChatErrorMessages.TryResolve,
        UserErrorMessages.TryResolve
    ];

    public Task<ServiceResult<IReadOnlyList<ConversationSummaryDto>>> GetConversationsAsync(CancellationToken ct = default)
        => ExecuteAsync(async () =>
        {
            var result = await GetAsync<IReadOnlyList<ConversationSummaryDto>>("api/conversations", ct);
            return result ?? [];
        }, ct);

    public Task<ServiceResult<ConversationDto>> CreateDirectAsync(
        CreateDirectConversationRequestDto request,
        CancellationToken ct = default)
        => ExecuteAsync(async () =>
        {
            var result = await PostAsync<CreateDirectConversationRequestDto, ConversationDto>(
                "api/conversations/direct",
                request,
                ct);

            return result ?? throw new InvalidOperationException("Failed to create conversation.");
        }, ct);

    public Task<ServiceResult<PagedResultDto<UserLookupDto>>> SearchUsersAsync(
        SearchUsersRequestDto request,
        CancellationToken ct = default)
        => ExecuteAsync(async () =>
        {
            var result = await GetAsync<PagedResultDto<UserLookupDto>>(
                AppendToQuery("api/users/search", request),
                ct);

            return result ?? new PagedResultDto<UserLookupDto>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }, ct);

    public Task<ServiceResult<PagedResultDto<MessageDto>>> GetMessagesAsync(
        Guid conversationId,
        CancellationToken ct = default)
        => ExecuteAsync(async () =>
        {
            var result = await GetAsync<PagedResultDto<MessageDto>>(
                AppendToQuery($"api/conversations/{conversationId}/messages", new GetMessagesRequestDto
                {
                    PageNumber = 1,
                    PageSize = 100
                }),
                ct);

            return result ?? new PagedResultDto<MessageDto>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 100
            };
        }, ct);
}
