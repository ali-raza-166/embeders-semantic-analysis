Important Steps to keep in mind for the project

Step 1: Publish image into registry and run it. 
This step includes repairing of SE code, creating docker file and build docker container. Push docker container to azure container registry and run it in ACI (Azure container Instance). We have to prepare the code to get input file, which soneone else can provide e.g. The code is in Pakistan and the client is providing input from USA.

*The code base is already provided to us. The link is as follows [Codebase] (https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2024-2025/tree/main/Source/MyCloudProjectSample). We have to edit it according to our project. 

Step 2: As our code is running and waiting for message. User send a message (upload input) to trigger the training process. Upload the input file in the blob storage (user uploads the training dataset as a zip file). 

*Our applications input will can be blob storage containing numerous files and output can be in a table storage. The input should be a searlized JSON. The app will call the JSON searliazer. 

