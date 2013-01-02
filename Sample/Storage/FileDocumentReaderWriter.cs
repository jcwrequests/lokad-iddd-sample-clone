using Sample.Projections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sample.Storage
{
    public class FileDocumentReaderWriter<TKey, TEntity> : IDocumentReader<TKey, TEntity>,
                                                             IDocumentWriter<TKey, TEntity>
    {
       readonly IDocumentStrategy _strategy;
        readonly string _folder;

        public FileDocumentReaderWriter(string directoryPath, IDocumentStrategy strategy)
        {
            _strategy = strategy;
            _folder = Path.Combine(directoryPath, strategy.GetEntityBucket<TEntity>());
        }

        public void InitIfNeeded()
        {
            Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            view = default(TEntity);
            try
            {
                var name = GetName(key);

                if (!File.Exists(name))
                    return false;

                using (var stream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream.Length == 0)
                        return false;
                    view = _strategy.Deserialize<TEntity>(stream);
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                // if file happened to be deleted between the moment of check and actual read.
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        string GetName(TKey key)
        {
            return Path.Combine(_folder, _strategy.GetEntityLocation<TEntity>(key));
        }

        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update,
            AddOrUpdateHint hint)
        {
            var name = GetName(key);

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);
 

                // we are locking this file.
                using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    byte[] initial = new byte[0];
                    TEntity result;
                    if (file.Length == 0)
                    {
                        result = addFactory();
                    }
                    else
                    {
                        using (var mem = new MemoryStream())
                        {
                            file.CopyTo(mem);
                            mem.Seek(0, SeekOrigin.Begin);
                            var entity = _strategy.Deserialize<TEntity>(mem);
                            initial = mem.ToArray();
                            result = update(entity);
                        }
                    }

                    // some serializers have nasty habbit of closing the
                    // underling stream
                    using (var mem = new MemoryStream())
                    {
                        _strategy.Serialize(result, mem);
                        var data = mem.ToArray();

                        if (!data.SequenceEqual(initial))
                        {
                            // upload only if we changed
                            file.Seek(0, SeekOrigin.Begin);
                            file.Write(data, 0, data.Length);
                            // truncate this file
                            file.SetLength(data.Length);
                        }
                    }

                    return result;
                }
            }
            catch (DirectoryNotFoundException)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }

        public bool TryDelete(TKey key)
        {
            var name = GetName(key);
            if (File.Exists(name))
            {
                File.Delete(name);
                return true;
            }
            return false;
        }

        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, Projections.AddOrUpdateHint hint = Projections.AddOrUpdateHint.ProbablyExists)
        {
            return AddOrUpdate(key, addFactory, update, Projections.AddOrUpdateHint.ProbablyExists);
        }
    }
}
