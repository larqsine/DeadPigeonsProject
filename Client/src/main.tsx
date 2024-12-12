import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.tsx';

// Wrap App with Router in main.tsx
createRoot(document.getElementById('root')!).render(
    <StrictMode>
            <App />
    </StrictMode>
);
