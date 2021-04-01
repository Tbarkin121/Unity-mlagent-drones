# Unity-mlagent-drones
Playing with ML Agents with Quadrotor type things. 

Used the drone models from : https://github.com/mbaske/ml-drone-collection

Trained :
Stabable flight with rewards given for maintaining a target veloicity vector. 

This allows for a higheractical control structure with path planning feeding velocity vectors to the control net. This also allows for user input to directly control the flight of the quadrotor

# Setup
Creat a new anaconda environment with python 3.7
conda create -n mlagents python=3.7
conda activate mlagents

# Installing mlagent requirements
https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Installation.md

pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
python -m pip install mlagents==0.25.0