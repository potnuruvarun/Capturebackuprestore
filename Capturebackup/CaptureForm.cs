using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restorebackup;
using Helpers;

namespace Capturebackup
{
    public partial class CaptureForm : Form
    {

        private FileSystemWatcher watcherChrome;
        private FileSystemWatcher watcherEdge;
        private FileSystemWatcher watcherFirefox;
        private HashSet<string> capturedUrls = new HashSet<string>();
        private const string lastCapturedTimeFile = "lastCapturedTime.txt";
        private long lastCapturedTime;

        public CaptureForm()
        {
            InitializeComponent();
            LoadLastCapturedTime();
            SetupFileWatcher();
        }
        //private void LoadLastCapturedTime()
        //{
        //    if (File.Exists(lastCapturedTimeFile))
        //    {
        //        long.TryParse(File.ReadAllText(lastCapturedTimeFile), out lastCapturedTime);
        //    }
        //}

        private void LoadLastCapturedTime()
        {
            if (File.Exists(lastCapturedTimeFile))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(lastCapturedTimeFile))
                    {
                        string content = reader.ReadLine();
                        if (long.TryParse(content, out long time))
                        {
                            lastCapturedTime = time;
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"File is in use: {ex.Message}");
                }
            }
        }

        private void SetupFileWatcher()
        {
            watcherChrome = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(ConstantUrls.chromeHistoryPath),
                Filter = "History",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcherChrome.Changed += OnHistoryChanged;
            watcherChrome.EnableRaisingEvents = true; // Start monitoring Chrome history

            watcherEdge = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(ConstantUrls.edgeHistoryPath),
                Filter = "History",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcherEdge.Changed += OnHistoryChanged;
            watcherEdge.EnableRaisingEvents = true;

            watcherFirefox = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(ConstantUrls.firefoxHistoryPath),
                Filter = "places.sqlite",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcherFirefox.Changed += OnHistoryChanged;
            watcherFirefox.EnableRaisingEvents = true;
        }

        private void OnHistoryChanged(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(ConstantUrls.chromeHistoryPath))
            {
                System.Threading.Thread.Sleep(1000); // Wait for file to be fully written
                CaptureAndBackupHistory(); // Automatically capture history when it changes
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //CaptureBrowserHistory(ConstantUrls.chromeHistoryPath, ConstantUrls.tempHistoryPath, ConstantUrls.backupFilePath, "Chrome");
            //CaptureBrowserHistory(ConstantUrls.firefoxHistoryPath, ConstantUrls.tempfirefoxPath, ConstantUrls.firefoxbackupFilePath, "Firefox");
            //    CaptureAndBackupHistory();
            //    //CaptureAndBackupcookies();
            //    //CaptureAndBackupPasswords();
            //    //BackupWebData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RestoreAllHistories();
            //RestoreCookies();
            //RestorePasswords();
        }

        //CaptureAndBAckupHistory
        private void CaptureAndBackupHistory()
        {
            try
            {
                CaptureBrowserHistory(ConstantUrls.chromeHistoryPath, ConstantUrls.tempHistoryPath, ConstantUrls.backupFilePath, "Chrome");
                CaptureBrowserHistory(ConstantUrls.edgeHistoryPath, ConstantUrls.tempedgePath, ConstantUrls.edgebackupFilePath, "Edge");
                CaptureBrowserHistory(ConstantUrls.firefoxHistoryPath, ConstantUrls.tempfirefoxPath, ConstantUrls.firefoxbackupFilePath, "Firefox");

                //CaptureBrowserHistory(ConstantUrls.chromeHistoryPath, ConstantUrls.tempHistoryPath, ConstantUrls.backupFilePath);
                //CaptureBrowserHistory(ConstantUrls.edgeHistoryPath, ConstantUrls.tempedgePath, ConstantUrls.edgebackupFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void CaptureBrowserHistory(string HistoryPath, string tempPath, string backupFilePath)
        //{
        //    try
        //    {
        //        Console.WriteLine(lastCapturedTimeFile);
        //        File.Copy(HistoryPath, tempPath, true);
        //        string connectionString = $"Data Source={tempPath};Version=3;";
        //        StringBuilder historyData = new StringBuilder();

        //        using (var connection = new SQLiteConnection(connectionString))
        //        {
        //            connection.Open();
        //            string query = $"SELECT url, title, last_visit_time FROM urls WHERE last_visit_time > {lastCapturedTime} ORDER BY last_visit_time ASC";

        //            using (var command = new SQLiteCommand(query, connection))
        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    string url = reader["url"].ToString();
        //                    string title = reader["title"].ToString();
        //                    long lastVisitTime = Convert.ToInt64(reader["last_visit_time"]);
        //                    DateTime lastVisit = ConvertWebkitTimestamp(lastVisitTime);

        //                    // Only capture new URLs
        //                    if (!capturedUrls.Contains(url))
        //                    {
        //                        capturedUrls.Add(url);
        //                        historyData.AppendLine($"Title: {title}\nURL: {url}\nVisited On: {lastVisit}\n--------------------\n");

        //                        // Update the last captured time
        //                        lastCapturedTime = lastVisitTime;
        //                    }
        //                }
        //            }
        //        }

        //        File.Delete(tempPath); // Cleanup temp file
        //                               // Display data in TextBox
        //        File.AppendAllText(backupFilePath, historyData.ToString());
        //        if (richTextBox1.InvokeRequired)
        //        {
        //            richTextBox1.Invoke(new Action(() =>
        //            {
        //                richTextBox1.AppendText(historyData.ToString());
        //            }));
        //        }
        //        else
        //        {
        //            richTextBox1.AppendText(historyData.ToString());
        //        }
        //        File.WriteAllText(lastCapturedTimeFile, lastCapturedTime.ToString());
        //        //MessageBox.Show("Backup completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void CaptureBrowserHistory(string historyPath, string tempPath, string backupFilePath, string browser)
        {
            try
            {
                Console.WriteLine(lastCapturedTimeFile);
                if (File.Exists(lastCapturedTimeFile))
                {
                    string lastTimeText = File.ReadAllText(lastCapturedTimeFile);
                    if (!long.TryParse(lastTimeText, out lastCapturedTime))
                    {
                        lastCapturedTime = 0; // Default if parsing fails
                    }
                }
                else
                {
                    lastCapturedTime = 0;
                }
                File.Copy(historyPath, tempPath, true); // Copy DB file to avoid locking issues
                string connectionString = $"Data Source={tempPath};Version=3;";
                StringBuilder historyData = new StringBuilder();

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "";
                    string lastVisitColumn = browser == "Firefox" ? "visit_date" : "last_visit_time";
                    if (browser == "Firefox")
                    {
                        query = $"SELECT url, title, visit_date FROM moz_places " +
                                $"JOIN moz_historyvisits ON moz_places.id = moz_historyvisits.place_id " +
                                $"WHERE visit_date > {lastCapturedTime * 1000000} ORDER BY visit_date ASC";
                    }
                    else
                    {
                        query = $"SELECT url, title, last_visit_time FROM urls WHERE last_visit_time > {lastCapturedTime} ORDER BY last_visit_time ASC";
                    }

                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string url = reader["url"].ToString();
                            string title = reader["title"].ToString();
                            long lastVisitTime = Convert.ToInt64(reader[lastVisitColumn]);

                            DateTime lastVisit = browser == "Firefox"
                                ? ConvertFirefoxTimestamp(lastVisitTime)
                                : ConvertWebkitTimestamp(lastVisitTime);

                            // Always update lastCapturedTime to the latest timestamp
                            if (lastVisitTime > lastCapturedTime)
                            {
                                lastCapturedTime = lastVisitTime;
                            }

                            // Only capture new URLs
                            if (!capturedUrls.Contains(url))
                            {
                                capturedUrls.Add(url);
                                historyData.AppendLine($"Title: {title}\nURL: {url}\nVisited On: {lastVisit}\n--------------------\n");
                            }

                        }
                    }
                }

                File.Delete(tempPath); // Cleanup temp file

                // Save data to backup
                File.AppendAllText(backupFilePath, historyData.ToString());

                // Update UI safely
                if (richTextBox1.InvokeRequired)
                {
                    richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(historyData.ToString())));
                    
                }
                else
                {
                    richTextBox1.AppendText(historyData.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                File.WriteAllText(lastCapturedTimeFile, lastCapturedTime.ToString());
            }

        }

        private DateTime ConvertFirefoxTimestamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp / 1000).LocalDateTime;
        }


        private static DateTime ConvertWebkitTimestamp(long webkitTimestamp)
        {
            DateTime epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddTicks(webkitTimestamp * 10).ToLocalTime();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }


      
        private void RestoreAllHistories()
        {
            string resultChrome = new HistoryRestorer("Chrome", ConstantUrls.chromeHistoryPath, ConstantUrls.backupFilePath, ConstantUrls.tempHistoryPath).RestoreHistory();
            string resultEdge = new HistoryRestorer("Edge", ConstantUrls.edgeHistoryPath, ConstantUrls.edgebackupFilePath, ConstantUrls.tempedgePath).RestoreHistory();
            string resultFirefox = new HistoryRestorer("Firefox", ConstantUrls.firefoxHistoryPath, ConstantUrls.firefoxbackupFilePath, ConstantUrls.tempfirefoxPath).RestoreHistory();

            MessageBox.Show("Sucessfully REstored");
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

        private void KillAllChromeProcesses()
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(3000);
                }
                catch { }
            }
        }

        private class HistoryEntry
        {
            public string Url { get; set; }
            public string Title { get; set; }
            public DateTime VisitTime { get; set; }
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



        private bool EnsureHistoryAccess()
        {
            try
            {
                // Try to open the file for reading and writing
                using (var fs = new FileStream(ConstantUrls.chromeHistoryPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private long ConvertToWebkitTimestamp(DateTime date)
        {
            var epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)((date.ToUniversalTime() - epoch).TotalMicroseconds);
        }
        private bool IsChromeRunning()
        {
            return Process.GetProcessesByName("chrome").Length > 0;
        }




        #region cookies
        private void CaptureAndBackupcookies()
        {
            try
            {
                // Ensure Chrome is closed before accessing the database
                if (IsChromeRunning())
                {
                    MessageBox.Show("Please close Chrome before capturing cookies.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Backup cookies from Chrome's SQLite database
                if (File.Exists(ConstantUrls.chromeCookiesPath))
                {
                    File.Copy(ConstantUrls.chromeCookiesPath, ConstantUrls.tempCookiesPath, true);
                }
                else
                {
                    MessageBox.Show("Chrome cookies file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string connectionString = $"Data Source={ConstantUrls.tempCookiesPath};Version=3;";
                StringBuilder cookiesData = new StringBuilder();

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT name, value, host_key, path, expires_utc FROM cookies ORDER BY creation_utc DESC LIMIT 50";

                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            string value = reader["value"].ToString();
                            string host_key = reader["host_key"].ToString();
                            string path = reader["path"].ToString();
                            long expiresUtc = Convert.ToInt64(reader["expires_utc"]);

                            cookiesData.AppendLine($"Name: {name}\nValue: {value}\nHost: {host_key}\nPath: {path}\nExpires: {expiresUtc}\n--------------------\n");
                        }
                    }
                }

                // Save cookies data to file
                File.WriteAllText(ConstantUrls.backupCookiesFilePath, cookiesData.ToString());
                richTextBox1.Text += cookiesData.ToString();
                File.Delete(ConstantUrls.tempCookiesPath); // Clean up temporary file

                MessageBox.Show("Cookies backed up successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoreCookies()
        {
            try
            {
                // Ensure Chrome is closed before restoring
                if (IsChromeRunning())
                {
                    MessageBox.Show("Please close Chrome before restoring cookies.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(ConstantUrls.backupCookiesFilePath))
                {
                    MessageBox.Show("No backup file found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] cookieLines = File.ReadAllLines(ConstantUrls.backupCookiesFilePath);
                if (cookieLines.Length == 0)
                {
                    MessageBox.Show("Backup file is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if the Cookies file exists
                if (!File.Exists(ConstantUrls.chromeCookiesPath))
                {
                    // If the cookies file does not exist, create a new one
                    CreateEmptyCookiesDatabase(ConstantUrls.chromeCookiesPath);
                    MessageBox.Show("Chrome cookies file not found. A new cookies database has been created.");
                }


                // Copy Chrome cookies to a temp file for modification
                File.Copy(ConstantUrls.chromeCookiesPath, ConstantUrls.tempCookiesPath, true);

                using (var connection = new SQLiteConnection($"Data Source={ConstantUrls.tempCookiesPath};Version=3;"))
                {
                    connection.Open();
                    CreateCookiesTable(connection);

                    using (var transaction = connection.BeginTransaction())
                    using (var command = new SQLiteCommand(connection))
                    {
                        foreach (var line in cookieLines)
                        {
                            if (line.StartsWith("Name: "))
                            {
                                string[] parts = line.Split(new[] { " Value: " }, StringSplitOptions.None);
                                if (parts.Length == 2)
                                {
                                    string name = parts[0].Substring(6); // Extract cookie name
                                    string valueAndRest = parts[1];

                                    // Further split into other parts like host, path, and expiration
                                    string[] valueParts = valueAndRest.Split(new[] { " Host: " }, StringSplitOptions.None);
                                    string value = valueParts[0];
                                    string hostKey = valueParts[1];

                                    string[] hostParts = valueParts[1].Split(new[] { " Path: " }, StringSplitOptions.None);
                                    string path = hostParts[1];
                                    string expires = hostParts[1].Split(new[] { " Expires: " }, StringSplitOptions.None)[1];

                                    long expiresUtc = long.TryParse(expires, out long result) ? result : 0;

                                    // Insert cookies data into the cookies table
                                    command.CommandText = "INSERT INTO cookies (name, value, host_key, path, expires_utc) VALUES (@name, @value, @host_key, @path, @expires_utc)";
                                    command.Parameters.AddWithValue("@name", name);
                                    command.Parameters.AddWithValue("@value", value);
                                    command.Parameters.AddWithValue("@host_key", hostKey);
                                    command.Parameters.AddWithValue("@path", path);
                                    command.Parameters.AddWithValue("@expires_utc", expiresUtc);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    connection.Close();
                }

                // Replace original cookies with modified ones
                File.Copy(ConstantUrls.tempCookiesPath, ConstantUrls.chromeCookiesPath, true);
                File.Delete(ConstantUrls.tempCookiesPath);

                MessageBox.Show("Cookies restored successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring cookies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateEmptyCookiesDatabase(string cookiesFilePath)
        {
            try
            {
                // Create a new SQLite database for cookies
                SQLiteConnection.CreateFile(cookiesFilePath);

                using (var connection = new SQLiteConnection($"Data Source={cookiesFilePath};Version=3;"))
                {
                    connection.Open();

                    // Create the necessary tables for cookies
                    CreateCookiesTable(connection);

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating cookies database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateCookiesTable(SQLiteConnection connection)
        {
            try
            {
                // Create the cookies table if it doesn't exist
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS cookies (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                value TEXT NOT NULL,
                host_key TEXT NOT NULL,
                path TEXT NOT NULL,
                expires_utc INTEGER NOT NULL
            );";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating cookies table: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion


        #region password
        private void CaptureAndBackupPasswords()

        {
            try
            {
                // Ensure Chrome is closed before capturing passwords
                if (IsChromeRunning())
                {
                    MessageBox.Show("Please close Chrome before capturing passwords.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //richTextBox1.Clear(); // Clear previous data

                // Ensure Login Data file exists
                if (!File.Exists(ConstantUrls.chromePasswordsPath))
                {
                    MessageBox.Show("Chrome Login Data file not found. Make sure Chrome is installed and used.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Copy Chrome's Login Data file to a temp file to prevent file locking
                string tempLoginDataPath = Path.Combine(Path.GetTempPath(), "LoginDataTemp.db");
                File.Copy(ConstantUrls.chromePasswordsPath, tempLoginDataPath, true);

                StringBuilder passwordData = new StringBuilder();
                byte[] encryptionKey = GetChromeEncryptionKey();

                // Connect to the copied SQLite database
                string connectionString = $"Data Source={tempLoginDataPath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Query to retrieve passwords from the `logins` table
                    string query = "SELECT origin_url, username_value, password_value, date_created, date_last_used FROM logins";

                    using (var command = new SQLiteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string originUrl = reader["origin_url"].ToString();
                            string username = reader["username_value"].ToString();
                            byte[] encryptedPassword = (byte[])reader["password_value"];
                            string password = DecryptChromePassword(encryptedPassword, encryptionKey);
                            //DateTime dateCreated = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(reader["date_created"])).DateTime;
                            //DateTime dateLastUsed = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(reader["date_last_used"])).DateTime;

                            passwordData.AppendLine($"Origin URL: {originUrl}\nUsername: {username}\nPassword: {password}");
                        }
                    }

                    connection.Close();
                }

                // Delete the temporary Login Data file after extraction
                File.Delete(tempLoginDataPath);

                // Write captured password data to backup file
                File.WriteAllText(ConstantUrls.backupPasswordsFilePath, passwordData.ToString());

                // Display data in TextBox for user to review
                richTextBox1.Text += passwordData.ToString();

                //MessageBox.Show("Passwords backup completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing passwords: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string DecryptChromePassword(byte[] encryptedData, byte[] masterKey)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length < 31)
                    return null;

                // Check for v10 prefix
                if (encryptedData[0] != 0x76 || encryptedData[1] != 0x31 || encryptedData[2] != 0x30)
                {
                    // Try legacy DPAPI decryption
                    try
                    {
                        return Encoding.UTF8.GetString(
                            ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser)
                        );
                    }
                    catch
                    {
                        return "Failed to decrypt (Legacy)";
                    }
                }

                byte[] nonce = new byte[12];
                byte[] ciphertext = new byte[encryptedData.Length - 31];
                byte[] tag = new byte[16];

                Buffer.BlockCopy(encryptedData, 3, nonce, 0, 12);
                Buffer.BlockCopy(encryptedData, 15, ciphertext, 0, encryptedData.Length - 31);
                Buffer.BlockCopy(encryptedData, encryptedData.Length - 16, tag, 0, 16);

                // Prepare output buffer
                byte[] plaintext = new byte[ciphertext.Length];

                // Perform decryption
                using (var aesGcm = new AesGcm(masterKey))
                {
                    aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
                    return Encoding.UTF8.GetString(plaintext);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Decryption error: {ex}");
                return $"Decryption failed: {ex.Message}";
            }
        }

        private byte[] GetChromeEncryptionKey()
        {
            try
            {
                // Read Local State file
                string localStateContent = File.ReadAllText(ConstantUrls.localStatePath);

                // Parse JSON
                var localState = JsonConvert.DeserializeObject<dynamic>(localStateContent);
                string base64Key = localState.os_crypt.encrypted_key;

                // Decode base64
                byte[] encryptedKey = Convert.FromBase64String(base64Key);

                // Remove DPAPI prefix
                byte[] keyWithoutPrefix = new byte[encryptedKey.Length - 5];
                Array.Copy(encryptedKey, 5, keyWithoutPrefix, 0, keyWithoutPrefix.Length);

                // Decrypt key using DPAPI
                return ProtectedData.Unprotect(keyWithoutPrefix, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting encryption key: {ex}");
                return null;
            }
        }

        private void RestorePasswordsFromBackup()
        {
            try
            {
                // Make sure Chrome is closed before attempting to restore
                if (IsChromeRunning())
                {
                    MessageBox.Show("Please close Chrome before restoring passwords.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Path for the backup file (assumed backup exists)

                if (!File.Exists(ConstantUrls.backupFilePath))
                {
                    MessageBox.Show("Backup file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Read backup data from file
                string backupData = File.ReadAllText(ConstantUrls.backupFilePath);

                // Copy the original login data to a temporary location for restoration
                //string chromeLoginDataPath = @"C:\Users\YourUserName\AppData\Local\Google\Chrome\User Data\Default\Login Data";
                File.Copy(ConstantUrls.chromePasswordsPath, ConstantUrls.tempLoginDataPath, true); // Copy to avoid file locking

                using (var connection = new SQLiteConnection($"Data Source={ConstantUrls.tempLoginDataPath};Version=3;"))
                {
                    connection.Open();

                    // Delete existing login records (Optional: You could skip this if you want to preserve existing passwords)
                    //string deleteQuery = "DELETE FROM logins";
                    //using (var deleteCommand = new SQLiteCommand(deleteQuery, connection))
                    //{
                    //    deleteCommand.ExecuteNonQuery();
                    //}

                    // Insert restored passwords from backup into the 'logins' table
                    // Ensure you parse the backup correctly based on how it was stored
                    string[] passwordLines = backupData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in passwordLines)
                    {
                        // Assuming backup has data in the format: Origin URL, Username, Password
                        var passwordDetails = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (passwordDetails.Length >= 3)
                        {
                            string originUrl = passwordDetails[0].Trim();
                            string username = passwordDetails[1].Trim();
                            string password = passwordDetails[2].Trim();

                            // Insert restored password into the logins table
                            string insertQuery = "INSERT INTO logins (origin_url, username_value, password_value) VALUES (@originUrl, @username, @password)";
                            using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@originUrl", originUrl);
                                insertCommand.Parameters.AddWithValue("@username", username);
                                insertCommand.Parameters.AddWithValue("@password", password);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Delete the temporary file after restoration
                File.Delete(ConstantUrls.tempLoginDataPath);

                MessageBox.Show("Passwords restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring passwords: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestorePasswords()
        {
            try
            {
                if (IsChromeRunning())
                {
                    MessageBox.Show("Please close Chrome before restoring passwords.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if Login Data file exists, if not create it
                if (!File.Exists(ConstantUrls.chromePasswordsPath))
                {
                    CreateNewLoginDataFile();
                }

                if (!File.Exists(ConstantUrls.backupPasswordsFilePath))
                {
                    MessageBox.Show("Backup file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read backup file
                string[] backupLines = File.ReadAllLines(ConstantUrls.backupPasswordsFilePath);
                var entries = ParseBackupFile(backupLines);

                if (!entries.Any())
                {
                    MessageBox.Show("No valid entries found in backup file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] encryptionKey = GetChromeEncryptionKey();
                if (encryptionKey == null)
                {
                    MessageBox.Show("Failed to get encryption key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string tempLoginDataPath = Path.Combine(Path.GetTempPath(), "LoginDataTemp.db");
                File.Copy(ConstantUrls.chromePasswordsPath, tempLoginDataPath, true);

                using (var connection = new SQLiteConnection($"Data Source={tempLoginDataPath};Version=3;"))
                {
                    connection.Open();

                    // Ensure the logins table exists
                    CreateLoginsTableIfNotExists(connection);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var entry in entries)
                            {
                                InsertOrUpdateLoginEntry(connection, entry, encryptionKey);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to restore entries: {ex.Message}");
                        }
                    }
                }

                try
                {
                    File.Copy(tempLoginDataPath, ConstantUrls.chromePasswordsPath, true);
                    MessageBox.Show("Passwords restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to update Chrome Login Data: {ex.Message}");
                }
                finally
                {
                    if (File.Exists(tempLoginDataPath))
                    {
                        File.Delete(tempLoginDataPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring passwords: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateNewLoginDataFile()
        {
            try
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(ConstantUrls.chromePasswordsPath));

                // Create a new SQLite database
                SQLiteConnection.CreateFile(ConstantUrls.chromePasswordsPath);

                using (var connection = new SQLiteConnection($"Data Source={ConstantUrls.chromePasswordsPath};Version=3;"))
                {
                    connection.Open();
                    CreateLoginsTableIfNotExists(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create new Login Data file: {ex.Message}");
            }
        }

        private void CreateLoginsTableIfNotExists(SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS logins (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    origin_url TEXT NOT NULL,
                    action_url TEXT NOT NULL,
                    username_element TEXT,
                    username_value TEXT,
                    password_element TEXT,
                    password_value BLOB,
                    submit_element TEXT,
                    signon_realm TEXT NOT NULL,
                    date_created INTEGER NOT NULL,
                    date_last_used INTEGER,
                    scheme INTEGER NOT NULL DEFAULT 0,
                    password_type INTEGER,
                    times_used INTEGER,
                    form_data BLOB,
                    display_name TEXT,
                    icon_url TEXT,
                    federation_url TEXT,
                    skip_zero_click INTEGER,
                    generation_upload_status INTEGER,
                    possible_username_pairs BLOB,
                    moving_blocked_for BLOB
                )";
                command.ExecuteNonQuery();

                // Create necessary indices
                command.CommandText = "CREATE INDEX IF NOT EXISTS logins_origin_url ON logins (origin_url)";
                command.ExecuteNonQuery();
            }
        }

        private void InsertOrUpdateLoginEntry(SQLiteConnection connection, LoginEntry entry, byte[] encryptionKey)
        {
            using (var command = new SQLiteCommand(connection))
            {
                // Modify the SQL query to include `blacklisted_by_user` column with a default value of 0 (not blacklisted)
                command.CommandText = @"
        INSERT OR REPLACE INTO logins (
            origin_url, 
            action_url, 
            username_element, 
            username_value,
            password_element, 
            password_value, 
            submit_element,
            signon_realm, 
            date_created, 
            date_last_used, 
            scheme,
            times_used,
            blacklisted_by_user
        )
        VALUES (
            @originUrl, 
            @originUrl, 
            '', 
            @username,
            '', 
            @password, 
            '',
            @originUrl, 
            @dateCreated, 
            @dateLastUsed, 
            0,
            1,
            0  -- Default to 0 (not blacklisted)
        )";

                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                command.Parameters.AddWithValue("@originUrl", entry.Url);
                command.Parameters.AddWithValue("@username", entry.Username);
                command.Parameters.AddWithValue("@password", EncryptPassword(entry.Password, encryptionKey));
                command.Parameters.AddWithValue("@dateCreated", currentTime);
                command.Parameters.AddWithValue("@dateLastUsed", currentTime);

                command.ExecuteNonQuery();
            }
        }

        private class LoginEntry
        {
            public string Url { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private List<LoginEntry> ParseBackupFile(string[] lines)
        {
            var entries = new List<LoginEntry>();
            LoginEntry currentEntry = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("Origin URL: "))
                {
                    currentEntry = new LoginEntry { Url = line.Substring(12) };
                }
                else if (line.StartsWith("Username: ") && currentEntry != null)
                {
                    currentEntry.Username = line.Substring(10);
                }
                else if (line.StartsWith("Password: ") && currentEntry != null)
                {
                    currentEntry.Password = line.Substring(10);
                    entries.Add(currentEntry);
                    currentEntry = null;
                }
            }

            return entries;
        }

        private byte[] EncryptPassword(string password, byte[] masterKey)
        {
            try
            {
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(password);
                byte[] nonce = new byte[12];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(nonce);
                }

                byte[] ciphertext = new byte[plaintextBytes.Length];
                byte[] tag = new byte[16];

                using (var aesGcm = new AesGcm(masterKey))
                {
                    aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
                }

                // Combine all parts: v10 + nonce + ciphertext + tag
                byte[] encryptedData = new byte[3 + nonce.Length + ciphertext.Length + tag.Length];
                encryptedData[0] = 0x76; // 'v'
                encryptedData[1] = 0x31; // '1'
                encryptedData[2] = 0x30; // '0'
                Buffer.BlockCopy(nonce, 0, encryptedData, 3, nonce.Length);
                Buffer.BlockCopy(ciphertext, 0, encryptedData, 3 + nonce.Length, ciphertext.Length);
                Buffer.BlockCopy(tag, 0, encryptedData, 3 + nonce.Length + ciphertext.Length, tag.Length);

                return encryptedData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to encrypt password: {ex.Message}");
            }
        }

        private void RestoreAllChromeData()
        {
            try
            {

                // Paths to restore Chrome's files
                string loginDataPath = $@"C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data\Profile 2\Login Data";
                string webDataPath = $@"C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data\Profile 2\Web Data";
                string cookiesPath = $@"C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data\Profile 2\Cookies";

                // Paths to the backup files
                string backupLoginDataFilePath = @"C:\Backup\LoginDataBackup.db";
                string backupWebDataFilePath = @"C:\Backup\WebDataBackup.db";
                string backupCookiesFilePath = @"C:\Backup\CookiesBackup.db";

                // Restore Login Data (passwords)
                File.Copy(backupLoginDataFilePath, loginDataPath, true);

                // Restore Web Data (autofill data)
                File.Copy(backupWebDataFilePath, webDataPath, true);

                // Restore Cookies (sessions, login state)
                File.Copy(backupCookiesFilePath, cookiesPath, true);

                // Notify user
                MessageBox.Show("Passwords, Autofill data, and Cookies restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion region
    }
}
