using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Storage.Blobs;
using Lab5.Data;
using Lab5.Models;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class CreateModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";
        private string containerName = "";
        private readonly PredictionDataContext _context;
        BlobContainerClient containerClient;


        public CreateModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult OnGet()
        {
            ViewData["PredictionID"] = new SelectList(_context.Predictions, "FileName", "Question", "Url");
            return Page();
        }

        [BindProperty]
        public Prediction Prediction { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {

        // Create the container and return a container client object

        Question question = Prediction.Question;

            if (question == Question.Earth)
            {
                containerName = earthContainerName;
            }
            else
            {
                containerName = computerContainerName;
            }

            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {

                var blockBlob = containerClient.GetBlobClient(file.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                var image = new Prediction
                {
                    Url = blockBlob.Uri.ToString(),
                    FileName = file.FileName,
                    Question = Prediction.Question
                };

                _context.Predictions.Add(image);
                await _context.SaveChangesAsync();
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
            }

            return RedirectToPage("./Index");
        }
    }
}
