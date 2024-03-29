using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Curiosity.DAL.EF.UnitTests.OnTransactionCompleted
{
    /// <summary>
    /// Tests for <see cref="CuriosityDataContext{T}"/> and <see cref="CuriosityDbTransaction"/> that checks event
    /// </summary>
    public class OnTransactionCompletedEventTests : IClassFixture<OnTransactionCompletedEventTestsFixture>
    {
        private readonly OnTransactionCompletedEventTestsFixture _fixture;

        /// <summary>
        /// </summary>
        public OnTransactionCompletedEventTests(OnTransactionCompletedEventTestsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        /// <summary>
        /// Checks that event will be fired when changes will be saved on context without explicit transaction creation.
        /// </summary>
        [Fact]
        public void DataContext_WithoutTransaction_OnSaveChanges_Triggered()
        {
            // arrange
            int syncTriggerCount;
            int asyncTriggerCount;
            using (var context = _fixture.CreateContext())
            {
                syncTriggerCount = 0;
                asyncTriggerCount = 0;

                context.OnTransactionCompleted += () => syncTriggerCount++;
                context.OnTransactionCompletedAsync += ct =>
                {
                    asyncTriggerCount++;
                    return Task.CompletedTask;
                };

                // act

                // event will be fired only on first save change
                context.SaveChanges();
                // after first fire event will be reset
                context.SaveChanges();
            }

            // assert
            syncTriggerCount.Should().Be(1, "we saved changes without explicit transaction creation");
            asyncTriggerCount.Should().Be(1, "we saved changes without explicit transaction creation");
        }

        /// <summary>
        /// Checks that event will be fired when changes will be saved on context without explicit transaction creation.
        /// </summary>
        [Fact]
        public async Task DataContext_WithoutTransaction_OnSaveChangesAsync_Triggered()
        {
            // arrange
            int syncTriggerCount;
            int asyncTriggerCount;
            await using (var context = _fixture.CreateContext())
            {
                syncTriggerCount = 0;
                asyncTriggerCount = 0;

                context.OnTransactionCompleted += () => syncTriggerCount++;
                context.OnTransactionCompletedAsync += ct =>
                {
                    asyncTriggerCount++;
                    return Task.CompletedTask;
                };

                // act

                // event will be fired only on first save change
                await context.SaveChangesAsync();
                // after first fire event will be reset
                await context.SaveChangesAsync();
            }

            // assert
            syncTriggerCount.Should().Be(1, "we saved changes without explicit transaction creation");
            asyncTriggerCount.Should().Be(1, "we saved changes without explicit transaction creation");
        }

        /// <summary>
        /// Checks that event will be fired when changes will be saved on context without explicit transaction creation.
        /// </summary>
        [Fact]
        public void DataContext_WithTransaction_WithoutCommit_OnSaveChanges_NotTriggered()
        {
            var syncTriggerCount = 0;
            var asyncTriggerCount = 0;

            // arrange
            using (var context = _fixture.CreateContext())
            using (var transaction = context.BeginTransaction())
            {
                context.OnTransactionCompleted += () => syncTriggerCount++;
                context.OnTransactionCompletedAsync += ct =>
                {
                    asyncTriggerCount++;
                    return Task.CompletedTask;
                };

                // act

                context.SaveChanges();
            }

            // assert
            syncTriggerCount.Should().Be(0, "we didnt' commit transaction");
            asyncTriggerCount.Should().Be(0, "we didnt' commit transaction");
        }

        /// <summary>
        /// Checks that event will be fired when changes will be saved on context without explicit transaction creation.
        /// </summary>
        [Fact]
        public void DataContext_WithTransaction_WithCommit_OnSaveChanges_Triggered()
        {
            var syncTriggerCount = 0;
            var asyncTriggerCount = 0;

            // arrange
            using (var context = _fixture.CreateContext())
            using (var transaction = context.BeginTransaction())
            {
                context.OnTransactionCompleted += () => syncTriggerCount++;
                context.OnTransactionCompletedAsync += ct =>
                {
                    asyncTriggerCount++;
                    return Task.CompletedTask;
                };

                // act

                context.SaveChanges();

                transaction.Commit();
            }

            // assert
            syncTriggerCount.Should().Be(1, "we committed transaction");
            asyncTriggerCount.Should().Be(1, "we committed transaction");
        }

        /// <summary>
        /// Checks that event will be fired when changes will be saved on context without explicit transaction creation.
        /// </summary>
        [Fact]
        public async Task DataContext_WithTransaction_WithCommitAsync_OnSaveChangesAsync_Triggered()
        {
            var syncTriggerCount = 0;
            var asyncTriggerCount = 0;

            CancellationToken cancellationToken = default;

            // arrange
            using (var context = _fixture.CreateContext())
            using (var transaction = await context.BeginTransactionAsync(cancellationToken: cancellationToken))
            {
                // act

                await context.SaveChangesAsync(cancellationToken);

                context.OnTransactionCompleted += () => syncTriggerCount++;
                context.OnTransactionCompletedAsync += ct =>
                {
                    asyncTriggerCount++;
                    return Task.CompletedTask;
                };

                await transaction.CommitAsync(cancellationToken);
            }

            // assert
            syncTriggerCount.Should().Be(1, "we committed transaction");
            asyncTriggerCount.Should().Be(1, "we committed transaction");
        }
    }
}
