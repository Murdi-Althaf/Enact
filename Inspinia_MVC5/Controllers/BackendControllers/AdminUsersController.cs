using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Enact.Models;
using System.Security.Cryptography;
using System.Net.Mail;

namespace Enact.Controllers.BackendControllers
{
    public class AdminUsersController : Controller
    {
        private EnactEntities db = new EnactEntities();


        public ActionResult Login(bool? incorrect)
        {
            if (incorrect == true)
            {
                ViewBag.scripCall = "showIncorrect();";
            }
            return View();
        }

        public ActionResult logon(string username, string password)
        {
            bool userexists = false;
            bool passwordverified = false;
            AdminUser admin = db.AdminUsers.Where(x => x.UserName == username).FirstOrDefault();
            if (admin != null)
            {
                userexists = db.AdminUsers.Where(x => x.UserName == username).Any();
                passwordverified = VerifyHash(password, "SHA256", null);
            }
           
            if (userexists && passwordverified)
            {
                return RedirectToAction("Index", "Audits");
            }
            else
            {
                return RedirectToAction("Login", "AdminUsers", new { @incorrect = true });
            }
        }

        public ActionResult forgot(String email)
        {
            if (db.AdminUsers.Where(x => x.Email == email).Any())
            {
                AdminUser auser = (from b in db.AdminUsers where b.Email.ToLower() == email.ToLower() select b).FirstOrDefault();
                sendemail(email,auser);
                return RedirectToAction("ForgotPassword", "AdminUsers", new { @incorrect = false });
            }
            else
            return RedirectToAction("ForgotPassword", "AdminUsers", new { @incorrect = true });
        }

        public void sendemail(String Email, AdminUser auser)
        {
            MailMessage mail = new MailMessage("eva.aphasia@gmail.com", Email);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            mail.Subject = "Password Reset for Enact website";
         
                Random rnd = new Random();
                int month = rnd.Next(111, 999);
                String temp = auser.UserName + month;
                String newPASS = ComputeHash(temp, "SHA256", null);
                auser.Password = newPASS;
                db.Entry(auser).State = EntityState.Modified;
                db.SaveChanges();
                mail.Body = "This is your new password : " + temp;
                client.Send(mail);
        }

        public ActionResult ForgotPassword(bool? incorrect)
        {
            if (incorrect == true)
            {
                ViewBag.scripCall = "showIncorrect();";
            }
            return View();
        }

        // GET: /AdminUsers/
        public async Task<ActionResult> Index()
        {
            return View(await db.AdminUsers.ToListAsync());
        }

        // GET: /AdminUsers/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = await db.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // GET: /AdminUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AdminUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ID,UserName,Password,Email")] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                if (adminUser.Password != "" && adminUser.Password != null && !db.AdminUsers.Where(x => x.UserName == adminUser.UserName).Any())
                {
                    adminUser.Password = ComputeHash(adminUser.Password, "SHA256", null);
                    db.AdminUsers.Add(adminUser);
                    await db.SaveChangesAsync();
                }
             
                return RedirectToAction("Index");
            }

            return View(adminUser);
        }

        public static string ComputeHash(string plainText,
                                   string hashAlgorithm,
                                   byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;

                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }


        public static bool VerifyHash(string plainText,
                                 string hashAlgorithm,
                                 string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            string expectedHashString =
                        ComputeHash(plainText, hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        // GET: /AdminUsers/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = await db.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            adminUser.Password = "";
            return View(adminUser);
        }

        // POST: /AdminUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ID,UserName,Password,Email")] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                if(adminUser.Password != "" && adminUser.Password != null && !db.AdminUsers.Where(x => x.UserName == adminUser.UserName).Any())
                {
                    adminUser.Password = ComputeHash(adminUser.Password, "SHA256", null);
                    db.Entry(adminUser).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                
                return RedirectToAction("Index");
            }
            return View(adminUser);
        }

        // GET: /AdminUsers/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = await db.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // POST: /AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            AdminUser adminUser = await db.AdminUsers.FindAsync(id);
            db.AdminUsers.Remove(adminUser);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
