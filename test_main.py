import unittest
from unittest.mock import patch, MagicMock
import os
from main import deliver_sandbox_dd_files_to_dev, get_environment_suffix, split_filename, extract_tla

class TestDeliverDataFunction(unittest.TestCase):
        
    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    @patch("main.extract_tla")
    def test_deliver_sandbox_dd_files_to_dev(self, mock_extract_tla, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-ips-bucketname",
            "name": "dd_IPS2411A_AA1_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "ips"
        mock_split_filename.return_value = ("dd_IPS2411A_AA1", "26112024_060148")
        mock_extract_tla.return_value = "IPS"

        # Mock storage client and bucket behavior
        mock_client_instance = MagicMock()
        mock_storage_client.return_value = mock_client_instance

        mock_source_bucket = MagicMock()
        mock_destination_bucket = MagicMock()
        mock_client_instance.bucket.side_effect = [mock_source_bucket, mock_destination_bucket]

        mock_source_blob = MagicMock()
        mock_source_bucket.blob.return_value = mock_source_blob

        # Call the function
        deliver_sandbox_dd_files_to_dev(data, context)

        # Assertions
        mock_get_environment_suffix.assert_called_once_with("ons-blaise-v2-dev-ips-bucketname")
        filename = os.path.splitext(data["name"])[0]
        mock_split_filename.assert_called_once_with(filename)
        mock_extract_tla.assert_called_once_with(prefix="dd_IPS2411A_AA1")

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-ips-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_IPS2411A_AA1_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_IPS2411A_AA1_ips_IPS_26112024_060148.zip"
        )