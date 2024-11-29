
import unittest
from unittest.mock import patch, MagicMock
from parameterized import parameterized

import os
from main import deliver_sandbox_dd_files_to_dev, get_environment_suffix, split_filename, extract_tla

class TestDeliverDataFunction(unittest.TestCase):
   
    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    @patch("main.extract_tla")
    def test_deliver_sandbox_dd_files_to_dev_for_IPS_DD_File(self, mock_extract_tla, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-ips-bucketname",
            "name": "dd_IPS2411A_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "ips"
        mock_split_filename.return_value = ("dd_IPS2411A", "26112024_060148")
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
        mock_extract_tla.assert_called_once_with(prefix="dd_IPS2411A")

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-ips-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_IPS2411A_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_IPS2411A_ips_IPS_26112024_060148.zip"
        )


    @patch("main.storage.Client")  
    @patch("main.get_environment_suffix")  
    @patch("main.split_filename") 
    @patch("main.extract_tla")
    def test_deliver_sandbox_dd_files_to_dev_for_FRS_DD_File(self, mock_extract_tla, mock_split_filename, mock_get_environment_suffix, mock_storage_client):
       
        data = {
            "bucket": "ons-blaise-v2-dev-loadtest2-bucketname",
            "name": "dd_FRS2411_AA1_26112024_060148.zip",
        }
        context = {}

        mock_get_environment_suffix.return_value = "loadtest2"
        mock_split_filename.return_value = ("dd_FRS2411_AA1", "26112024_060148")
        mock_extract_tla.return_value = "FRS"

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
        mock_extract_tla.assert_called_once_with(prefix="dd_FRS2411_AA1")

        mock_storage_client.assert_called_once()
        mock_client_instance.bucket.assert_any_call("ons-blaise-v2-dev-loadtest2-bucketname")

        mock_source_bucket.blob.assert_called_once_with("dd_FRS2411_AA1_26112024_060148.zip")
        mock_source_bucket.copy_blob.assert_called_once_with(
            mock_source_blob, mock_destination_bucket, "dd_FRS2411_AA1_loadtest2_FRS_26112024_060148.zip"
        )

    @patch('main.logging.info') 
    def test_deliver_sandbox_dd_files_should_not_work_for_non_dd_files(self, mock_info,):
       
        data = {
            "bucket": "ons-blaise-v2-dev-loadtest2-bucketname",
            "name": "mi_FRS2411_AA1_26112024_060148.zip",
        }
        context = {}

        deliver_sandbox_dd_files_to_dev(data, context)

        mock_info.assert_called_once_with("Non-dd file received, no data delivery needed")

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
    ])
    def test_split_filename(self, filename, expected_prefix, expected_suffix) :
       
        
        received_prefix , received_suffix = split_filename(filename)
       
        assert received_prefix == expected_prefix
        assert received_suffix == expected_suffix


    @parameterized.expand([
        ("dd_FRS2411_AA1", "FRS"),
        ("dd_IPS2411A", "IPS"),
        ("dd_LMS2412A_AA1", "LMS"),
    ])
    def test_extract_tla(self, prefix, expected_tla):
       
        received_tla = extract_tla(prefix)
        assert received_tla == expected_tla

    @parameterized.expand([
        ("ons-blaise-v2-dev-rr3", "rr3"),
        ("ons-blaise-v2-dev-sj03", "sj03"),
        ("ons-blaise-v2-dev-loadtest2", "loadtest2"),
    ])
    def test_get_environment_suffix(self, environment, expected_env_suffix):
       
        received_env_suffix = get_environment_suffix(environment)
        assert received_env_suffix == expected_env_suffix