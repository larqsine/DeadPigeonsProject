import React from 'react';
import Navbar from './components/Navbar';
import BoxGrid from './components/BoxGrid';
import styles from './App.module.css';

const App: React.FC = () => {
    return (
        <div className={styles.app}>
            <Navbar />
            <BoxGrid />
        </div>
    );
};

export default App;
