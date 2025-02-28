import unittest
from unittest.mock import patch, MagicMock
from parameterized import parameterized

import os
from main import deliver_sandbox_dd_files_to_dev, get_environment_suffix, split_filename

class TestDeliverDataFunction(unittest.TestCase):
   
    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    def test_deliver_sandbox_dd_files_to_dev_for_IPS_DD_File(self, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-ips-bucketname",
            "name": "dd_IPS2411A_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "ips"
        mock_split_filename.return_value = ("dd_IPS2411A", "26112024_060148")

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

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-ips-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_IPS2411A_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_IPS2411A_sandbox_ips_26112024_060148.zip"
        )


    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    def test_deliver_sandbox_dd_files_to_dev_for_FRS_DD_File(self, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-loadtest2-bucketname",
            "name": "dd_FRS2411_AA1_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "loadtest2"
        mock_split_filename.return_value = ("dd_FRS2411_AA1", "26112024_060148")

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
        mock_get_environment_suffix.assert_called_once_with("ons-blaise-v2-dev-loadtest2-bucketname")
        filename = os.path.splitext(data["name"])[0]
        mock_split_filename.assert_called_once_with(filename)

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-loadtest2-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_FRS2411_AA1_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_FRS2411_AA1_sandbox_loadtest2_26112024_060148.zip"
        )

    


    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    def test_deliver_sandbox_dd_files_to_dev_for_IPS_DD_File(self, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-loadtest2-bucketname",
            "name": "dd_IPS2413A_AA1_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "loadtest2"
        mock_split_filename.return_value = ("dd_IPS2413A_AA1", "26112024_060148")

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
        mock_get_environment_suffix.assert_called_once_with("ons-blaise-v2-dev-loadtest2-bucketname")
        filename = os.path.splitext(data["name"])[0]
        mock_split_filename.assert_called_once_with(filename)

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-loadtest2-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_IPS2413A_AA1_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_IPS2413A_AA1_sandbox_loadtest2_26112024_060148.zip"
        )

    @patch('main.logging.info') 
    def test_deliver_sandbox_dd_files_should_not_work_for_non_dd_files(self, mock_info,):
       
        data = {
            "bucket": "ons-blaise-v2-dev-loadtest2-bucketname",
            "name": "mi_FRS2411_AA1_26112024_060148.zip",
        }
        context = {}

        deliver_sandbox_dd_files_to_dev(data, context)

        assert 3 == mock_info.call_count
        last_logged_message = mock_info.call_args_list[-1][0][0]
        
        self.assertEqual(last_logged_message, "Non-dd file received, no data delivery needed")

    @patch('main.logging.error')
    def test_deliver_sandbox_dd_files_to_dev_should_raise_exception_for_null_data_trigger(self, mock_error, ):
       
        data = None
        context = {}

        deliver_sandbox_dd_files_to_dev(data, context)

        # Assertions
        mock_error.assert_called_once_with(f"An error occured while trying to run the data-delivery-function. Exception: Not a valid request object")
    
    
    @parameterized.expand([
        ("dd_FRS2411_AA1_26112024_060148", "dd_FRS2411_AA1","26112024_060148"),
        ("dd_IPS4211A_26112024_060149", "dd_IPS4211A","26112024_060149"),
        ("dd_LMS2412A_AA1_26112024_060149", "dd_LMS2412A_AA1","26112024_060149"),
        ("dd_IPS2413A_AA1_26112024_060148", "dd_IPS2413A_AA1","26112024_060148"),
    ])
    def test_split_filename(self, filename, expected_prefix, expected_suffix) :
       
        
        received_prefix , received_suffix = split_filename(filename)
       
        assert received_prefix == expected_prefix
        assert received_suffix == expected_suffix


    @parameterized.expand([
        ("ons-blaise-v2-dev-rr3-nifi", "rr3"),
        ("ons-blaise-v2-dev-sj03-nifi", "sj03"),
        ("ons-blaise-v2-dev-loadtest2-nifi", "loadtest2"),
    ])
    def test_get_environment_suffix(self, bucket_name, expected_env_suffix):
       
        received_env_suffix = get_environment_suffix(bucket_name)
        assert received_env_suffix == expected_env_suffix