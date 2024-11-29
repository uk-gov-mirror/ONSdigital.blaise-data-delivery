from google.cloud import storage
import logging
import os
from cloud_logging import setup_logger

setup_logger()

def deliver_sandbox_dd_files_to_dev(data, _context):

    try:
        if(data == None or data == {}):
            raise ValueError("Not a valid request object")
        
        bucket_name = data["bucket"]
        file_name = data["name"]

        logging.info(f"Sandbox Data Delivery Process triggered:")
        logging.info(f"File received: {file_name}")

        if("mi" not in file_name and "dd" in file_name):

            storage_client = storage.Client()

            destination_bucket_name = "ons-blaise-v2-dev-nifi"

            env_suffix = get_environment_suffix(bucket_name)
            filename, fileExtension = os.path.splitext(file_name)  #removes extension only
            
            prefix, suffix = split_filename(filename) 

            new_file_name = f"{prefix}_{env_suffix}_{extract_tla(prefix=prefix)}_{suffix}{fileExtension}"

            print(f"New file name {new_file_name}")

            # Get source and destination bucket objects
            source_bucket = storage_client.bucket(bucket_name)
            destination_bucket = storage_client.bucket(destination_bucket_name)

            source_blob = source_bucket.blob(file_name)

            new_blob = source_bucket.copy_blob(source_blob, destination_bucket, new_file_name)

            print(f"File {file_name} copied to {destination_bucket_name}/{new_file_name}") 
        else:
            logging.info("Non-dd file received, no data delivery needed")
            return
    except Exception as e:
        logging.error(f"An error occured while trying to run the data-delivery-function. Exception: {e}")
        return

def get_environment_suffix(bucket_name):
    parts = bucket_name.split("-")
    env_suffix = parts[len(parts)-2]
    return env_suffix

def split_filename(filename):
    filename = ''.join(filename)
    
    parts = filename.rsplit("_",2)
    if len(parts) == 3:
        prefix = parts[0]
        suffix = "_".join(parts[1:])
        return prefix, suffix
    return None, None

def extract_tla(prefix):
    prefix = ''.join(prefix)
    parts = prefix.split("_")
    return parts[1][:3]