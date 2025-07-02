import React, { useState } from 'react';
import { publicApiClient } from '../../services/api';

interface ImageUploadProps {
  onImageUploaded: (imageUrl: string) => void;
  currentImageUrl?: string;
  className?: string;
}

const ImageUpload: React.FC<ImageUploadProps> = ({ 
  onImageUploaded, 
  currentImageUrl, 
  className = "" 
}) => {
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validar tipo de archivo
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setError('Solo se permiten archivos de imagen (jpg, jpeg, png, gif, webp)');
      return;
    }

    // Validar tamaño (5MB)
    if (file.size > 5 * 1024 * 1024) {
      setError('El archivo es demasiado grande. Máximo 5MB');
      return;
    }

    // Mostrar preview
    const reader = new FileReader();
    reader.onload = (e) => {
      setPreviewUrl(e.target?.result as string);
    };
    reader.readAsDataURL(file);

    setIsUploading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await publicApiClient.post('/api/Upload/producto', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      const imageUrl = response.data.url;
      onImageUploaded(imageUrl);
      setError(null);
    } catch (err: any) {
      console.error('Error al subir imagen:', err);
      setError(err.response?.data?.mensaje || 'Error al subir la imagen');
    } finally {
      setIsUploading(false);
    }
  };

  const handleRemoveImage = () => {
    setPreviewUrl(null);
    onImageUploaded('');
    setError(null);
  };

  return (
    <div className={`image-upload ${className}`}>
      <div className="upload-area">
        {previewUrl || currentImageUrl ? (
          <div className="image-preview">
            <img 
              src={previewUrl || currentImageUrl} 
              alt="Preview" 
              className="preview-image"
            />
            <div className="image-actions">
              <label className="btn-secondary">
                Cambiar imagen
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleFileChange}
                  style={{ display: 'none' }}
                  disabled={isUploading}
                />
              </label>
              <button
                type="button"
                onClick={handleRemoveImage}
                className="btn-danger"
                disabled={isUploading}
              >
                Eliminar
              </button>
            </div>
          </div>
        ) : (
          <label className="upload-placeholder">
            <div className="upload-icon">📷</div>
            <div className="upload-text">
              {isUploading ? 'Subiendo imagen...' : 'Haz clic para subir una imagen'}
            </div>
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              style={{ display: 'none' }}
              disabled={isUploading}
            />
          </label>
        )}
      </div>

      {error && (
        <div className="error-message">
          {error}
        </div>
      )}

      {isUploading && (
        <div className="upload-progress">
          <div className="loading-spinner"></div>
          <span>Subiendo imagen...</span>
        </div>
      )}

      <style jsx>{`
        .image-upload {
          width: 100%;
        }

        .upload-area {
          border: 2px dashed #ddd;
          border-radius: 8px;
          padding: 20px;
          text-align: center;
          transition: border-color 0.3s ease;
        }

        .upload-area:hover {
          border-color: #007bff;
        }

        .upload-placeholder {
          cursor: pointer;
          display: block;
        }

        .upload-icon {
          font-size: 48px;
          margin-bottom: 10px;
        }

        .upload-text {
          color: #666;
          font-size: 14px;
        }

        .image-preview {
          position: relative;
        }

        .preview-image {
          max-width: 100%;
          max-height: 200px;
          border-radius: 4px;
          margin-bottom: 10px;
        }

        .image-actions {
          display: flex;
          gap: 10px;
          justify-content: center;
        }

        .btn-secondary, .btn-danger {
          padding: 8px 16px;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-size: 14px;
          transition: background-color 0.3s ease;
        }

        .btn-secondary {
          background-color: #6c757d;
          color: white;
        }

        .btn-secondary:hover {
          background-color: #5a6268;
        }

        .btn-danger {
          background-color: #dc3545;
          color: white;
        }

        .btn-danger:hover {
          background-color: #c82333;
        }

        .btn-secondary:disabled, .btn-danger:disabled {
          opacity: 0.6;
          cursor: not-allowed;
        }

        .error-message {
          color: #dc3545;
          font-size: 14px;
          margin-top: 10px;
        }

        .upload-progress {
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 10px;
          margin-top: 10px;
          color: #007bff;
        }

        .loading-spinner {
          width: 20px;
          height: 20px;
          border: 2px solid #f3f3f3;
          border-top: 2px solid #007bff;
          border-radius: 50%;
          animation: spin 1s linear infinite;
        }

        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
};

export default ImageUpload; 