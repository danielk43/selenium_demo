#!/usr/bin/env python

from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
from selenium import webdriver
import os
import math
import shutil
import pyotp
import time

start = time.time()

# Delete browser cache if exists
cache_dir = "./data/test"

if os.path.isdir(cache_dir):
    shutil.rmtree(cache_dir)

options = webdriver.ChromeOptions()
options.binary_location = r"./chrome-linux64/chrome"
options.add_argument("--remote-debugging-port=9222")
options.add_argument("--start-maximized")
options.add_argument("--no-sandbox")
options.add_argument("--disable-dev-shm-using")
options.add_argument("--disable-extensions")
options.add_argument("--disable-gpu")
options.add_argument("--disable-infobars")
options.add_argument("--disable-setuid-sandbox")
options.add_argument("--user-data-dir={}".format(cache_dir))
options.add_experimental_option(
    "prefs", {"profile.managed_default_content_settings.images": 2})
service = Service("./chromedriver-linux64/chromedriver")
driver = webdriver.Chrome(options=options, service=service)

# Log into GitHub account with 2FA
driver.get("https://github.com/login")

driver.find_element(
    By.ID, "login_field").send_keys(
    os.environ["GITHUB_USERNAME"])
driver.find_element(By.ID, "password").send_keys(os.environ["GITHUB_PASSWORD"])
commit = driver.find_element(By.NAME, "commit")
commit.click()

github_totp = pyotp.TOTP(os.environ["GITHUB_TOTP"])
github_token = github_totp.now()

driver.find_element(By.NAME, "app_otp").send_keys(github_token)
print("GitHub Token = " + github_token)

test_resources = {
    "backend.tf": ["s3"],
    "ec2.tf": [
        "latest-rhel8",
        "latest-win22",
        "ec2_instance"
    ],
    "outputs.tf": [
        "ami_id_rhel8",
        "ami_id_win22",
        "vpc_rhel_id",
        "vpc_win_id",
        "vpc_rhel_priv_subnet_id1",
        "vpc_rhel_priv_subnet_id2",
        "vpc_rhel_priv_subnet_id3",
        "vpc_rhel_priv_subnet_id4",
        "vpc_rhel_pub_subnet_id1",
        "vpc_rhel_pub_subnet_id2",
        "vpc_rhel_pub_subnet_id3",
        "vpc_rhel_pub_subnet_id4",
        "vpc_win_priv_subnet_id1",
        "vpc_win_priv_subnet_id2",
        "vpc_win_priv_subnet_id3",
        "vpc_win_priv_subnet_id4",
        "vpc_win_pub_subnet_id1",
        "vpc_win_pub_subnet_id2",
        "vpc_win_pub_subnet_id3",
        "vpc_win_pub_subnet_id4",
        "vpc_rhel_security_group_id",
        "vpc_win_security_group_id"
    ],
    "provider.tf": ["aws"],
    "variables.tf": [
        "aws_region",
        "azs",
        "enable_nat_gateway",
        "enable_vpn_gateway",
        "manage_default_network_acl",
        "vpcs",
        "ec2s"
    ],
    "vpc.tf": ["vpc"]
}

for tf_file, res_list in test_resources.items():
    # Navigate to main repo menu
    avatar_button = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable(
            (By.CSS_SELECTOR,
             "button[aria-label='Open user navigation menu']"))
    )
    avatar_button.click()

    repositories = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable(
            (By.CSS_SELECTOR, "a[href='/{}?tab=repositories']".format(
                os.environ["GITHUB_USERNAME"])))
    )
    repositories.click()

    # Open Terraform project
    driver.implicitly_wait(5)
    repos_filter = driver.find_element(By.ID, "your-repos-filter")
    repos_filter.send_keys("zscaler-tf")
    zscaler_repo = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable(
            (By.LINK_TEXT, "zscaler-tf"))
    )
    zscaler_repo.click()

    # Search file, load page
    go_to_file = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable(
            (By.CSS_SELECTOR, "input[aria-label='Go to file']"))
    )
    go_to_file.send_keys(tf_file)
    go_to_file_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable(
            (By.CSS_SELECTOR,
             "div[data-component='ActionList.Item--DividerContainer']"))
    )
    go_to_file_link.click()

    # Verify file content contains resources
    driver.implicitly_wait(5)
    tf_content = driver.find_element(
        By.CSS_SELECTOR,
        "textarea[id='read-only-cursor-text-area']").get_attribute("value")

    for resource in res_list:
        assert resource in tf_content
        print("found {} in {}".format(resource, tf_file))

# Print success and exit
print("Terraform test passed in %s seconds" % round((time.time() - start), 3))
