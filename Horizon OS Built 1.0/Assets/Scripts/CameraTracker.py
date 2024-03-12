import cv2
import mediapipe as mp

class HandTracking:
    def __init__(self, camera, width, height):
        self.camera = camera
        self.width = width
        self.height = height

        # Inisialisasi MediaPipe
        self.mp_hands = mp.solutions.hands
        self.hands = self.mp_hands.Hands(static_image_mode=False, max_num_hands=1, min_detection_confidence=0.9, min_tracking_confidence=0.9)

        # Variabel untuk menyimpan posisi landmark sebelumnya
        self.prev_middle_finger_tip = None
        self.prev_index_finger_tip = None
        self.prev_ring_finger_tip = None
        self.prev_pinky_finger_tip = None

    def set_camera(self):
        self.camera.set(3, self.width)
        self.camera.set(4, self.height)

    def track_hand(self):
        # Ambil frame dari kamera
        success, img = self.camera.read()
        if not success:
            pass

        # Ubah gambar menjadi RGB (MediaPipe menggunakan gambar RGB)
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

        # Deteksi tangan
        results = self.hands.process(img_rgb)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                # Ambil nilai koordinat x, y, z dari landmark
                landmarks = []
                for lm in hand_landmarks.landmark:
                    landmarks.append((lm.x, lm.y, lm.z))

                # Cek apakah gesture pinch terdeteksi
                thumb_tip = hand_landmarks.landmark[self.mp_hands.HandLandmark.THUMB_TIP]
                index_finger_tip = hand_landmarks.landmark[self.mp_hands.HandLandmark.INDEX_FINGER_TIP]
                middle_finger_tip = hand_landmarks.landmark[self.mp_hands.HandLandmark.MIDDLE_FINGER_TIP]
                ring_finger_tip = hand_landmarks.landmark[self.mp_hands.HandLandmark.RING_FINGER_TIP]
                pinky_tip = hand_landmarks.landmark[self.mp_hands.HandLandmark.PINKY_TIP]

                thumb_middle_distance = ((thumb_tip.x - middle_finger_tip.x) ** 2 + (thumb_tip.y - middle_finger_tip.y) ** 2) ** 0.5
                thumb_index_distance = ((thumb_tip.x - index_finger_tip.x) ** 2 + (thumb_tip.y - index_finger_tip.y) ** 2) ** 0.5
                thumb_ring_distance = ((thumb_tip.x - ring_finger_tip.x) ** 2 + (thumb_tip.y - ring_finger_tip.y) ** 2) ** 0.5
                thumb_pinky_distance = ((thumb_tip.x - pinky_tip.x) ** 2 + (thumb_tip.y - pinky_tip.y) ** 2) ** 0.5

                # Tentukan threshold untuk gesture pinch
                pinch_threshold = 0.1  # Sesuaikan threshold sesuai kebutuhan

                # MIDDLE FINGER
                if thumb_middle_distance < pinch_threshold:
                # Jika posisi landmark jari tengah sebelumnya telah tersedia
                    if self.prev_middle_finger_tip:
                        # Hitung perpindahan landmark jari tengah
                        delta_x = round((middle_finger_tip.x - self.prev_middle_finger_tip.x) * 100)
                        delta_y = round((middle_finger_tip.y - self.prev_middle_finger_tip.y) * 100)
                        delta_z = round((middle_finger_tip.z - self.prev_middle_finger_tip.z) * 100)

                        # Cetak perpindahan landmark jari tengah
                        # print(f"Perpindahan Jari Tengah: dx={delta_x}, dy={delta_y}, dz={delta_z}")
                        return "PINCH!"
                    # Simpan posisi landmark jari tengah saat ini sebagai posisi sebelumnya
                    self.prev_middle_finger_tip = middle_finger_tip


        # cv2.imshow('Gambar', img)

# camera = cv2.VideoCapture(0)
# myHand = HandTracking(camera, 1280, 720)
# myHand.set_camera()
# while True:
#     myHand.track_hand()
#     # Tekan 'q' untuk keluar dari loop
#     if cv2.waitKey(1) & 0xFF == ord('q'):
#         break

# camera.release()
# cv2.destroyAllWindows()
