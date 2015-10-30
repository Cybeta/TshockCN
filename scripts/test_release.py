import subprocess
import shutil
import os.path
import zipfile

def generate_release():
    zip = zipfile.ZipFile("tshock_release.zip", "r")
    zip.extractall()

def generate_configs():
    subprocess.call(['/usr/local/bin/mono', 'TerrariaServer.exe', '-dump'])
    if not os.path.isfile('ConfigDescriptions.txt') or not os.path.isfile('PermissionsDescriptions.txt') or not os.path.isfile('ServerSideConfigDescriptions.txt') or not os.path.isfile('RestDescriptions.txt'):
        raise CalledProcessError(1)

if __name__ == '__main__':
  generate_release()
  generate_configs()