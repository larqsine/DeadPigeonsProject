import React, { useState } from "react";
import axios from "axios";
import styles from "./ChangePasswordPage.module.css";

const ChangePasswordPage: React.FC = () => {
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [verifyNewPassword, setVerifyNewPassword] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");


    const handleSubmit = async () => {
        if (newPassword !== verifyNewPassword) {
            setError("New passwords do not match.");
            return;
        }

        try {
            const token = localStorage.getItem("token");

            const response = await axios.post(
                "https://server-587187818392.europe-west1.run.app/api/account/change-password",
                { currentPassword, newPassword },
                {
                    headers: { Authorization: `Bearer ${token}` },
                }
            );

            setSuccess(response.data.message);
            setError("");

            // Redirect to the Play Page after a successful password change
            setTimeout(() => {
                window.location.href = "/";
            }, 2000);
        } catch (error: any) {
            setError(error.response?.data?.message || "Password change failed.");
            setSuccess("");
        }
    };

    return (
        <div className={styles.container}>
            <h2>Change Password</h2>
            {error && <p className={styles.error}>{error}</p>}
            {success && <p className={styles.success}>{success}</p>}
            <div className={styles.form}>
                <input
                    type="password"
                    placeholder="Current Password"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                />
                <input
                    type="password"
                    placeholder="New Password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                />
                <input
                    type="password"
                    placeholder="Verify New Password"
                    value={verifyNewPassword}
                    onChange={(e) => setVerifyNewPassword(e.target.value)}
                />
                <button onClick={handleSubmit}>Save</button>
            </div>
        </div>
    );
};

export default ChangePasswordPage;
