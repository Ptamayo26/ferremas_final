import React from 'react';

interface OpenCarritoButtonProps {
  onOpen: () => void;
  className?: string;
}

const OpenCarritoButton: React.FC<OpenCarritoButtonProps> = ({ onOpen, className = '' }) => {
  return (
    <button
      onClick={onOpen}
      className={`btn-primary ${className}`}
    >
      🛒 Ver Carrito
    </button>
  );
};

export default OpenCarritoButton; 