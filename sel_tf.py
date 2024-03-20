#!/home/daniel/selenium/bin/python

import time

start = time.time()

import os
import math
import shutil
import pyotp

from selenium import webdriver
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By

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

# Log into GitHub account with 2FA
driver = webdriver.Chrome(options=options, service=service)
driver.get("https://github.com/login")

commit = driver.find_element(By.NAME, "commit")
wait_commit = WebDriverWait(driver, timeout=2)
wait_commit.until(lambda d: commit.is_displayed())

driver.find_element(
    By.ID, "login_field").send_keys(
        os.environ["GITHUB_USERNAME"])
driver.find_element(By.ID, "password").send_keys(os.environ["GITHUB_PASSWORD"])
commit.click()

github_totp = pyotp.TOTP(os.environ["GITHUB_TOTP"])
github_token = github_totp.now()

driver.find_element(By.NAME, "app_otp").send_keys(github_token)
print("GitHub Token = " + github_token)

# Navigate to main repo menu
avatar_button = driver.find_element(
    By.CSS_SELECTOR,
    "button[aria-label='Open user account menu']")
avatar_button.click()

repositories = WebDriverWait(driver, 10).until(
    EC.element_to_be_clickable(
        (By.CSS_SELECTOR, "a[href='/{}?tab=repositories']".format(
            os.environ["GITHUB_USERNAME"])))
)
repositories.click()

# Open Terraform project
driver.implicitly_wait(5)
zscaler_repo = driver.find_element(By.LINK_TEXT, "zscaler-tf")
zscaler_repo.click()

# Search file, load page
go_to_file = WebDriverWait(driver, 20).until(
    EC.element_to_be_clickable(
        (By.CSS_SELECTOR, "input[aria-label='Go to file']"))
)
go_to_file.send_keys("ec2.tf")
go_to_file_link = driver.find_element(
    By.CSS_SELECTOR, "div[data-component='ActionList.Item--DividerContainer']"
)
go_to_file_link.click()

# Verify file content contains resources
ec2_tf_content = driver.find_element(
    By.CSS_SELECTOR,
    "textarea[id='read-only-cursor-text-area']").get_attribute("value")

for resource in ["latest-rhel8", "latest-win22", "ec2_instance"]:
    assert resource in ec2_tf_content

# Print success and exit
print("Terraform test passed in %s seconds" % round((time.time() - start), 3))
