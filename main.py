from google.cloud import storage
import os

def deliver_sandbox_dd_files_to_dev(data, context):
     # Extract bucket and file information from the event
    bucket_name = data["bucket"]
    file_name = data["name"]

    # Create the storage client
    storage_client = storage.Client()

    # Set the destination bucket name
    destination_bucket_name = "ons-blaise-v2-dev-nifi"


    # sample file name ons-blaise-v2-dev-ips-nifi/dd_IPS2411A_26112024_060148.zip
    # Modify the filename - ensure to swap out "loadtest2" with a given string

    env_suffix = get_environment_suffix(bucket_name)
    filename, fileExtension = os.path.splitext(file_name)  #removes extension only
    
    prefix, suffix = split_filename(filename) 

    new_file_name = f"{prefix}_{env_suffix}_{extract_tla(prefix=prefix)}_{suffix}{fileExtension}"

    print(f"New file name {new_file_name}")

    # Get source and destination bucket objects
    source_bucket = storage_client.bucket(bucket_name)
    destination_bucket = storage_client.bucket(destination_bucket_name)

    # Get the source blob (file)
    source_blob = source_bucket.blob(file_name)

    # Copy the blob to the destination bucket with the new name
    new_blob = source_bucket.copy_blob(
        source_blob, destination_bucket, new_file_name
    )

    print(f"File {file_name} copied to {destination_bucket_name}/{new_file_name}") 


def get_environment_suffix(environment):
    parts = environment.split("-")
    return parts[len(parts)-1]

def split_filename(filename):
    print("in func")
    print(filename)
    filename = ''.join(filename)
    if "IPS" not in filename:
        print("NON-IPS block")
        parts = filename.rsplit("_", 2)
        if len(parts) == 3:
            prefix = "_".join(parts[:2])
            suffix = parts[2]  
            return prefix, suffix
    if "IPS" in filename:
        print("IPS block")
        parts = filename.rsplit("_", 1)
        if len(parts) ==2:
            prefix = "_".join(parts[:1])
            suffix = parts[1]  
            return prefix, suffix
    return None, None

def extract_tla(prefix):
    parts = prefix.split("_")
    if len(parts) >= 2:
        return parts[1][:3]  # Extract the 'TLA' part
    return None