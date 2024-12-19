import React from "react";
import { useAtom } from "jotai";
import axios from "axios";
import styles from "./ChangePasswordPage.module.css";
import {
    showCurrentPasswordAtom,
    showNewPasswordAtom,
    showVerifyNewPasswordAtom,
} from "./PagesJotaiStore.ts";

const ChangePasswordPage: React.FC = () => {
    const [currentPassword, setCurrentPassword] = React.useState("");
    const [newPassword, setNewPassword] = React.useState("");
    const [verifyNewPassword, setVerifyNewPassword] = React.useState("");
    const [error, setError] = React.useState("");
    const [success, setSuccess] = React.useState("");

    // Jotai atoms for toggling password visibility
    const [showCurrentPassword, setShowCurrentPassword] = useAtom(showCurrentPasswordAtom);
    const [showNewPassword, setShowNewPassword] = useAtom(showNewPasswordAtom);
    const [showVerifyNewPassword, setShowVerifyNewPassword] = useAtom(showVerifyNewPasswordAtom);

    const handleSubmit = async () => {
        if (newPassword !== verifyNewPassword) {
            setError("New passwords do not match.");
            return;
        }

        try {
            const token = localStorage.getItem("token");

            const response = await axios.post(
                "https://dead-pigeons-backend-587187818392.europe-west1.run.app/api/account/change-password",
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
                <div className={styles.passwordInput}>
                    <input
                        type={showCurrentPassword ? "text" : "password"}
                        placeholder="Current Password"
                        value={currentPassword}
                        onChange={(e) => setCurrentPassword(e.target.value)}
                    />
                    <button
                        type="button"
                        className={styles.toggleButton}
                        onClick={() => setShowCurrentPassword((prev) => !prev)}
                    >
                        {showCurrentPassword ? "Hide" : "Show"}
                    </button>
                </div>
                <div className={styles.passwordInput}>
                    <input
                        type={showNewPassword ? "text" : "password"}
                        placeholder="New Password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                    />
                    <button
                        type="button"
                        className={styles.toggleButton}
                        onClick={() => setShowNewPassword((prev) => !prev)}
                    >
                        {showNewPassword ? "Hide" : "Show"}
                    </button>
                </div>
                <div className={styles.passwordInput}>
                    <input
                        type={showVerifyNewPassword ? "text" : "password"}
                        placeholder="Verify New Password"
                        value={verifyNewPassword}
                        onChange={(e) => setVerifyNewPassword(e.target.value)}
                    />
                    <button
                        type="button"
                        className={styles.toggleButton}
                        onClick={() => setShowVerifyNewPassword((prev) => !prev)}
                    >
                        {showVerifyNewPassword ? "Hide" : "Show"}
                    </button>
                </div>
                <button onClick={handleSubmit}>Save</button>
            </div>
        </div>
    );
};

export default ChangePasswordPage;
