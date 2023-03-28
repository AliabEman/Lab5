using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;
using System.Linq.Expressions;

namespace Lab5.Pages.Predictions
{
    public class DeleteModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";
        private string containerName = "";
        private BlobContainerClient _blobContainerClient;



        private readonly Lab5.Data.PredictionDataContext _context;

        public DeleteModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        [BindProperty]
        public Prediction Prediction { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Predictions == null)
            {
                return NotFound();
            }

            Prediction = await _context.Predictions.FirstOrDefaultAsync(model => model.PredictionID == id);

            if (Prediction == null)
            {
                return NotFound();
            }
            else 
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var foundID = await _context.Predictions.FindAsync(id);


            if (foundID != null) //
            {
                return NotFound();
            }


            // Return the correct blob container according to the user's question
            Question question = Prediction.Question;

            if (question == Question.Computer)
                containerName = computerContainerName;
            else
                containerName = earthContainerName;

            try
            {
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return NotFound();
            }

            if (Prediction != null)
            {
                try
                {
                    _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    if (await _blobContainerClient.GetBlobClient(Prediction.FileName).ExistsAsync())
                    {
                        await _blobContainerClient.GetBlobClient(Prediction.FileName).DeleteAsync();
                    }
                    _context.Predictions.Remove(Prediction);
                    await _context.SaveChangesAsync();


                }
                catch (RequestFailedException)
                {
                    return RedirectToPage("/Error");
                }

            }
            return RedirectToPage("./Index");
        }
    }
}
