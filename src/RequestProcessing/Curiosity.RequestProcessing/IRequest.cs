﻿using System.Globalization;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Запрос, который надо обработать.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// ИД запроса.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Культура запроса.
        /// </summary>
        CultureInfo RequestCulture { get; }
    }
}
