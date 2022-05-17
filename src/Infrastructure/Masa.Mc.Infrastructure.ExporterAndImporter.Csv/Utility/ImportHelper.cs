﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.ExporterAndImporter.Csv.Utility
{
    /// <summary>
    ///     导入辅助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImportHelper<T> : IDisposable where T : class, new()
    {
        /// <summary>
        /// </summary>
        /// <param name="filePath"></param>
        public ImportHelper(string filePath = null)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        public ImportHelper(Stream stream)
        {
            Stream = stream;
        }

        /// <summary>
        ///     导入文件路径
        /// </summary>
        protected string FilePath { get; set; }

        /// <summary>
        ///     导入结果
        /// </summary>
        internal ImportResult<T> ImportResult { get; set; }

        /// <summary>
        ///     文件流
        /// </summary>
        protected Stream Stream { get; set; }

        /// <summary>
        ///     导入模型
        /// </summary>
        /// <returns></returns>
        public Task<ImportResult<T>> Import(string filePath = null)
        {
            if (!string.IsNullOrWhiteSpace(filePath)) FilePath = filePath;

            ImportResult = new ImportResult<T>();
            try
            {
                if (Stream == null)
                {
                    CheckImportFile(FilePath);
                    Stream = new FileStream(FilePath, FileMode.Open);
                }

                using (var reader = new StreamReader(Stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<AutoMap<T>>();
                    var result = csv.GetRecords<T>();
                    ImportResult.Data = result.ToList();
                    return Task.FromResult(ImportResult);
                }
            }
            catch (Exception ex)
            {
                ImportResult.Exception = ex;
            }
            finally
            {
                ((IDisposable)Stream)?.Dispose();
            }
            return Task.FromResult(ImportResult);
        }

        /// <summary>
        ///     导入模型
        /// </summary>
        /// <returns></returns>
        public Task<ImportResult<T>> DynamicImport(string filePath = null)
        {
            if (!string.IsNullOrWhiteSpace(filePath)) FilePath = filePath;

            ImportResult = new ImportResult<T>();
            try
            {
                if (Stream == null)
                {
                    CheckImportFile(FilePath);
                    Stream = new FileStream(FilePath, FileMode.Open);
                }

                using (var reader = new StreamReader(Stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var result = csv.GetRecords<T>();
                    ImportResult.Data = result.ToList();
                    return Task.FromResult(ImportResult);
                }
            }
            catch (Exception ex)
            {
                ImportResult.Exception = ex;
            }
            finally
            {
                ((IDisposable)Stream)?.Dispose();
            }
            return Task.FromResult(ImportResult);
        }

        /// <summary>
        ///     导出模板
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> GenerateTemplateByte()
        {
            var properties = typeof(T).GetSortedPropertyInfos();

            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.Configuration.HasHeaderRecord = true;
                #region header
                foreach (var prop in properties)
                {
                    var name = prop.Name;
                    var headerAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ExporterHeaderAttribute>();
                    if (headerAttribute != null)
                    {
                        name = headerAttribute.DisplayName ?? prop.GetDisplayName() ?? prop.Name;
                    }
                    var importAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
                    if (importAttribute != null)
                    {
                        name = importAttribute.Name ?? prop.GetDisplayName() ?? prop.Name;
                    }
                    csv.WriteField(name);
                }
                csv.NextRecord();
                #endregion

                writer.Flush();
                ms.Position = 0;
                return Task.FromResult(ms.ToArray());
            }
        }

        /// <summary>
        ///     检查导入文件路径
        /// </summary>
        /// <exception cref="ArgumentException">文件路径不能为空! - filePath</exception>
        private static void CheckImportFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException(Resource.FilePathCannotBeEmpty, nameof(filePath));

            //TODO:在Docker容器中存在文件路径找不到问题，暂时先注释掉
            //if (!File.Exists(filePath))
            //{
            //    throw new ImportException("导入文件不存在!");
            //}
        }
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            FilePath = null;
            ImportResult = null;
            Stream = null;
            GC.Collect();
        }


    }
}
