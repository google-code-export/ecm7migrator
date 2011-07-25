﻿namespace ECM7.Migrator.Providers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	/// <summary>
	/// Поддержка форматов "TAB", "COL", "NAME" для экранирования идентификаторов
	/// </summary>
	public class SqlFormatter : IFormatProvider, ICustomFormatter
	{
		/// <summary>
		/// Функция, выполняющая экранирование идентификаторов
		/// </summary>
		protected readonly Func<object, string> converter;

		/// <summary>
		/// Инициализация
		/// </summary>
		/// <param name="converter">Функция, выполняющая экранирование идентификаторов</param>
		public SqlFormatter(Func<object, string> converter)
		{
			Require.IsNotNull(converter, "Не задана функция экранирования идентификаторов");
			this.converter = converter;
		}

		/// <summary>
		/// Returns an object that provides formatting services for the specified type.
		/// </summary>
		/// <returns>
		/// An instance of the object specified by <paramref name="formatType"/>, if the <see cref="T:System.IFormatProvider"/> implementation can supply that type of object; otherwise, null.
		/// </returns>
		/// <param name="formatType">An object that specifies the type of format object to return. </param><filterpriority>1</filterpriority>
		public object GetFormat(Type formatType)
		{
			return formatType == typeof(ICustomFormatter) ? this : null;
		}

		/// <summary>
		/// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
		/// </summary>
		/// <returns>
		/// The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.
		/// </returns>
		/// <param name="format">A format string containing formatting specifications. </param><param name="arg">An object to format. </param><param name="formatProvider">An object that supplies format information about the current instance. </param><filterpriority>2</filterpriority>
		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			string ufmt = (format ?? string.Empty).ToUpper(CultureInfo.InvariantCulture);

			try
			{
				if (arg is IEnumerable<object> && ufmt == "COLS")
				{
					IEnumerable<object> collection = arg as IEnumerable<object>;
					return collection.Select(converter).ToCommaSeparatedString();
				}

				if (ufmt == "NAME")
				{
					return this.converter(arg);
				}

				return HandleOtherFormats(format, arg);
			}
			catch (FormatException ex)
			{
				throw new FormatException(string.Format("The format of '{0}' is invalid.", format), ex);
			}
		}

		/// <summary>
		/// Обработка формата стандартным провайдером форматирования
		/// </summary>
		/// <param name="fmt">Формат</param>
		/// <param name="arg">Форматируемый объект</param>
		private static string HandleOtherFormats(string fmt, object arg)
		{
			if (arg is IFormattable)
			{
				return (arg as IFormattable).ToString(fmt, CultureInfo.CurrentCulture);
			}

			if (arg != null)
			{
				return arg.ToString();
			}
			return string.Empty;
		}
	}
}