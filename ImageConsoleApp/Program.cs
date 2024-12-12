using Microsoft.Data.SqlClient;

class Program
{
    private static string connectionString = "Server=.;Database=TestDb;User ID=sa;Password=sasa@123;TrustServerCertificate=True;";

    static void Main(string[] args)
    {
        string savedImagePath = "D:\\";
        //string savedImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "retrieved_image.jpg");

        Console.WriteLine("Enter your Imagepath save to database");
        string imagePath = Console.ReadLine()!;
        Console.WriteLine("1. Saving image to database...");
        int imageId = SaveImageToDatabase(imagePath);
        if (imageId > 0)
        {
            Console.WriteLine($"Image successfully saved to database with ID = {imageId}");

            Console.WriteLine("\n2. Retrieving image from database...");
            RetrieveImageFromDatabase(imageId, savedImagePath);

            Console.WriteLine($"Image successfully retrieved from database and saved as {savedImagePath}");
        }
        else
        {
            Console.WriteLine("Failed to save image to the database.");
        }
    }

    public static int SaveImageToDatabase(string imagePath)
    {
        try
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            string imageName = Path.GetFileName(imagePath);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Images (ImageName, ImageData) OUTPUT INSERTED.Id VALUES (@ImageName, @ImageData)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ImageName", imageName);
                    command.Parameters.AddWithValue("@ImageData", imageData);

                    int newImageId = (int)command.ExecuteScalar();
                    return newImageId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving image to the database: " + ex.Message);
            return -1;
        }
    }

    public static void RetrieveImageFromDatabase(int imageId, string outputPath)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ImageData FROM Images WHERE Id = @ImageId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ImageId", imageId);

                    byte[] imageData = command.ExecuteScalar() as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        File.WriteAllBytes(outputPath, imageData);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving image from the database: " + ex.Message);
        }
    }

}