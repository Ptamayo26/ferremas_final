import React from 'react';

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = 'Confirmar',
  cancelText = 'Cancelar',
  type = 'danger'
}) => {
  if (!isOpen) return null;

  const getTypeStyles = () => {
    switch (type) {
      case 'danger':
        return {
          icon: '⚠️',
          confirmButton: 'btn-danger',
          titleColor: 'text-red-600'
        };
      case 'warning':
        return {
          icon: '⚠️',
          confirmButton: 'btn-warning',
          titleColor: 'text-yellow-600'
        };
      case 'info':
        return {
          icon: 'ℹ️',
          confirmButton: 'btn-primary',
          titleColor: 'text-blue-600'
        };
      default:
        return {
          icon: '⚠️',
          confirmButton: 'btn-danger',
          titleColor: 'text-red-600'
        };
    }
  };

  const styles = getTypeStyles();

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md mx-4">
        <div className="flex items-center mb-4">
          <span className="text-2xl mr-3">{styles.icon}</span>
          <h2 className={`text-xl font-bold ${styles.titleColor}`}>
            {title}
          </h2>
        </div>

        <p className="text-gray-700 mb-6 whitespace-pre-line">
          {message}
        </p>

        <div className="flex justify-end space-x-3">
          <button
            onClick={onClose}
            className="btn-secondary"
          >
            {cancelText}
          </button>
          <button
            onClick={() => {
              onConfirm();
              onClose();
            }}
            className={styles.confirmButton}
          >
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmDialog; 