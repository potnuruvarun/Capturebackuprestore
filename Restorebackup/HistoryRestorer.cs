using System.Data.SQLite;

namespace Restorebackup
{
    public class HistoryRestorer
    {

        private string chromeHistoryPath;
        private string backupFilePath;
        private string tempHistoryPath;

        public HistoryRestorer(string chromePath, string backupPath, string tempPath)
        {
            chromeHistoryPath = chromePath;
            backupFilePath = backupPath;
            tempHistoryPath = tempPath;
        }

        public string RestoreHistory()
        {
            try
            {
                if (IsChromeRunning())
                {
                    return "Please close Chrome before restoring history.";
                }

                if (!File.Exists(backupFilePath))
                {
                    return "No backup file found!";
                }

                // Read the backup content
                string backupContent = File.ReadAllText(backupFilePath);
                var entries = ParseBackupContent(backupContent);

                if (!entries.Any())
                {
                    return "No valid entries found in backup file.";
                }

                // Make sure we have write access to Chrome history
                if (!EnsureHistoryAccess())
                {
                    return "Cannot access Chrome history file. Check permissions.";
                }

                // Copy Chrome history to temp location
                File.Copy(chromeHistoryPath, tempHistoryPath, true);

                using (var connection = new SQLiteConnection($"Data Source={tempHistoryPath};Version=3;"))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var entry in entries)
                            {
                                long urlId = InsertHistoryEntry(connection, entry);
                                InsertVisit(connection, urlId, entry.VisitTime);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return $"Failed to insert entries: {ex.Message}";
                        }
                    }
                }

                // Replace the original history file
                try
                {
                    KillAllChromeProcesses();
                    Thread.Sleep(1000);
                    File.Copy(tempHistoryPath, chromeHistoryPath, true);
                    return "History restored successfully!";
                }
                catch (Exception ex)
                {
                    return $"Failed to copy history back to Chrome: {ex.Message}";
                }
                finally
                {
                    if (File.Exists(tempHistoryPath))
                    {
                        File.Delete(tempHistoryPath);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error restoring history: {ex.Message}";
            }
        }
        private bool IsChromeRunning()
        {
            return System.Diagnostics.Process.GetProcessesByName("chrome").Any();
        }

        private void KillAllChromeProcesses()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }
        }

        private bool EnsureHistoryAccess()
        {
            return File.Exists(chromeHistoryPath);
        }


        private List<HistoryEntry> ParseBackupContent(string content)
        {
            var entries = new List<HistoryEntry>();
            var lines = content.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            HistoryEntry currentEntry = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("Title: "))
                {
                    currentEntry = new HistoryEntry();
                    currentEntry.Title = line.Substring(7).Trim();
                }
                else if (line.StartsWith("URL: ") && currentEntry != null)
                {
                    currentEntry.Url = line.Substring(5).Trim();
                }
                else if (line.StartsWith("Visited On: ") && currentEntry != null)
                {
                    if (DateTime.TryParse(line.Substring(11).Trim(), out DateTime visitTime))
                    {
                        currentEntry.VisitTime = visitTime;
                        entries.Add(currentEntry);
                        currentEntry = null;
                    }
                }
            }

            return entries;
        }

        private long InsertHistoryEntry(SQLiteConnection connection, HistoryEntry entry)
        {
            using (var command = new SQLiteCommand(connection))
            {
                // First check if URL exists
                command.CommandText = "SELECT id FROM urls WHERE url = @url";
                command.Parameters.AddWithValue("@url", entry.Url);
                var existingId = command.ExecuteScalar();

                if (existingId != null)
                {
                    return Convert.ToInt64(existingId);
                }

                // If URL doesn't exist, insert it
                command.CommandText = @"
                INSERT INTO urls (url, title, last_visit_time, visit_count, typed_count, hidden)
                VALUES (@url, @title, @lastVisitTime, @visitCount, @typedCount, 0);
                SELECT last_insert_rowid();";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@url", entry.Url);
                command.Parameters.AddWithValue("@title", entry.Title);
                command.Parameters.AddWithValue("@lastVisitTime", ConvertToWebkitTimestamp(entry.VisitTime));
                command.Parameters.AddWithValue("@visitCount", 1);
                command.Parameters.AddWithValue("@typedCount", 1);

                return Convert.ToInt64(command.ExecuteScalar());
            }
        }

        private void InsertVisit(SQLiteConnection connection, long urlId, DateTime visitTime)
        {
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"
                INSERT INTO visits (url, visit_time, from_visit, transition, visit_duration)
                VALUES (@urlId, @visitTime, 0, 805306368, 0)";

                command.Parameters.AddWithValue("@urlId", urlId);
                command.Parameters.AddWithValue("@visitTime", ConvertToWebkitTimestamp(visitTime));

                command.ExecuteNonQuery();
            }
        }

        private long ConvertToWebkitTimestamp(DateTime date)
        {
            var epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)((date.ToUniversalTime() - epoch).TotalMicroseconds);
        }
    }

    public class HistoryEntry
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime VisitTime { get; set; }
    }
}

