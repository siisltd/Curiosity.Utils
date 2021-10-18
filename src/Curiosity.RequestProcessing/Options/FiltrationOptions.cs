using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Настройки фильтрации запросов.
    /// </summary>
    public class FiltrationOptions :
        ILoggableOptions,
        IValidatableOptions
    {
        /// <summary>
        /// Id клиентов для фильтрации (only).
        /// </summary>
        public long[]? OnlyClientIds { get; set; }
        
        /// <summary>
        /// Id проектов для фильтрации (only).
        /// </summary>
        public long[]? OnlyProjectIds { get; set; }
        
        /// <summary>
        /// Делитель Id проекта. Используется вместе с <see cref="ProjectIdReminderOfDivision"/>.
        /// Позволяет балансировать обработку проектов между воркерами.
        /// </summary>
        public long? ProjectIdDivider { get; set; }
        
        /// <summary>
        /// Остаток от деления Id проекта. Используется вместе с <see cref="ProjectIdDivider"/>.
        /// Позволяет балансировать обработку проектов между воркерами.
        /// </summary>
        public long? ProjectIdReminderOfDivision { get; set; }

        /// <summary>
        /// Id клиентов, задачи по которым не будут браться.
        /// </summary>
        public long[]? ExcludeClientIds { get; set; }
        
        /// <summary>
        /// Id проектов задачи по которым не будут браться.
        /// </summary>
        public long[]? ExcludeProjectIds { get; set; }
              
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            ValidateIds(errors, OnlyClientIds, ExcludeClientIds, nameof(OnlyClientIds), nameof(ExcludeClientIds));
            ValidateIds(errors, OnlyProjectIds, ExcludeProjectIds, nameof(OnlyProjectIds), nameof(ExcludeProjectIds));
            if (ProjectIdDivider != null && OnlyProjectIds != null)
            {
                errors.AddError(nameof(OnlyClientIds), $"Нельзя указывать ИД проекта в {nameof(OnlyProjectIds)}, т.к. выбран режиме остаток от деления Id проекта");
            }

            errors.AddErrorIf(ProjectIdDivider < 1, nameof(ProjectIdDivider), "должен быть больше 0");
            errors.AddErrorIf(ProjectIdReminderOfDivision < 0, nameof(ProjectIdReminderOfDivision), "не может быть отрицательным числом");

            return errors;
        }

        private void ValidateIds(
            ConfigurationValidationErrorCollection errorBuilder,
            IReadOnlyList<long>? onlyList,
            IReadOnlyList<long>? excludeList,
            string onlyFieldName,
            string excludeFieldName)
        {
            if (onlyList != null && onlyList.Count > 0 && excludeList != null && excludeList.Count > 0)
            {
                var onlyIds = new HashSet<long>(onlyList);
                var excludeIds = new HashSet<long>(excludeList);
                onlyIds.IntersectWith(excludeIds);
                errorBuilder.AddErrorIf(onlyIds.Count > 0, onlyFieldName, $"Id \"{String.Join(", ", onlyIds)}\" не могут быть одновременно в \"{onlyFieldName}\" и \"{excludeFieldName}\"");
            }
        }
    }
}
