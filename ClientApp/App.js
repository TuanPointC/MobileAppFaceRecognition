import React, { useState, useEffect, useRef } from 'react';
import { StyleSheet, Text, View, TouchableOpacity, Alert } from 'react-native';
import { Camera } from 'expo-camera';
import ModalView from './ModalView';
import axios from 'axios';
import { manipulateAsync, FlipType, SaveFormat } from 'expo-image-manipulator';


export default function App() {
  const [hasPermission, setHasPermission] = useState(null);
  const [type, setType] = useState(Camera.Constants.Type.front);
  const [showCamera, setShowCamera] = useState(false)
  const [image, setImage] = useState(null)
  const [modalVisible, setModalVisible] = useState(false);
  const cameraRef = useRef(null)

  useEffect(() => {
    (async () => {
      const { status } = await Camera.requestCameraPermissionsAsync();
      setHasPermission(status === 'granted');
    })();
  }, []);

  const _rotate90andFlip = async (uri) => {
    const manipResult = await manipulateAsync(
      uri,
      [
        { flip: FlipType.Horizontal },
      ],
//      { compress: 1, format: SaveFormat.PNG }
    );
    console.log(manipResult)
    return manipResult;
  };

  const takePhoto = async () => {
    if (cameraRef) {
      console.log("take pictrue")
    }
    try {
      let photo = await cameraRef.current.takePictureAsync({
        
      })
      console.log(photo.uri)
      photo = _rotate90andFlip(photo.uri)
      return photo
    }
    catch (e) {
      console.log(e)
    }
  }


  const uploadImage = async (uri) => {
    const photo = {
      uri: uri,
      name: 'image.jpg',
      type: 'image/jpg'
    };
    const formData = new FormData();

    formData.append('file', photo);

    axios.post('http://192.168.0.111:5001/FaceRecognition', formData, {
      headers: {
        "Content-Type": "application/json", Accept: "application/json"
      }
    }
    ).then(
      response => {
        setImage(response.data)
        setModalVisible(true)
      }
    ).catch(
      error => {
        console.log(error)
        Alert.alert("Cannot detect face")
      }
    )

  }

  if (hasPermission === null) {
    return <View />;
  }
  if (hasPermission === false) {
    return <Text>No access to camera</Text>;
  }

  return (
    <View style={{ flex: 1 }}>
      {
        showCamera ?
          <View style={styles.container}>
            <Camera
              ref={cameraRef}
              style={styles.camera}
              type={type}
            />
            <View style={styles.buttonContainer}>
              <TouchableOpacity
                style={styles.button}
                onPress={() => {
                  setType(
                    type === Camera.Constants.Type.front
                      ? Camera.Constants.Type.back
                      : Camera.Constants.Type.front
                  );
                }}>
                <Text style={styles.text}> Flip </Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={styles.button}
                onPress={async () => {
                  var a = await takePhoto()
                  uploadImage(a.uri)
                }}>
                <Text style={styles.text}> Photo </Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={styles.button}
                onPress={() => setShowCamera(false)}>
                <Text style={styles.text}> Exit </Text>
              </TouchableOpacity>
            </View>
            <ModalView modalVisible={modalVisible} setModalVisible={setModalVisible} image={image} />
          </View>
          :

          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <TouchableOpacity
              style={{ ...styles.button }}
              onPress={() => setShowCamera(true)}>
              <Text style={styles.text}> Camera </Text>
            </TouchableOpacity>
          </View>

      }
    </View>
  )


}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'space-around'
  },
  camera: {
    width: 480,
    height: 720,
    borderRadius: 10,
    overflow: 'hidden'
  },
  buttonContainer: {
    flexDirection: 'row'
  },
  button: {
    backgroundColor: 'green',
    paddingHorizontal: 30,
    paddingVertical: 20,
    margin: 10,
    borderRadius: 10,
  },
  text: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: 20
  }
}); 