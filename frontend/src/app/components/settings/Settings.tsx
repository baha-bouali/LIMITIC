import React, { useEffect, useState } from 'react'
import { useGetSettingsQuery, useUpdateSettingsMutation, useTestSmtpMutation } from './api/settingsApiSlice'

export default function Settings() {
  const { data, isLoading } = useGetSettingsQuery()
  const [updateSettings, { isLoading: isSaving }] = useUpdateSettingsMutation()
  const [testSmtp, { isLoading: isTesting }] = useTestSmtpMutation()

  const [labName, setLabName] = useState('')

  useEffect(() => {
    if (data) setLabName(data.identity.labName)
  }, [data])

  const handleSave = async () => {
    await updateSettings({
      identity: { labName, labSlogan: '', contactEmail: '', address: '', phone: '', logoUrl: null },
      smtp: { host: '', port: 25, username: '', password: null, useTls: false },
    })
  }

  const handleTest = async () => {
    await testSmtp({ testEmail: 'test@example.com' })
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">Lab Settings</h2>
      <div className="mb-4">
        <label className="block text-sm font-medium">Lab name</label>
        <input value={labName} onChange={(e) => setLabName(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="flex gap-2">
        <button onClick={handleSave} disabled={isSaving} className="px-4 py-2 bg-blue-600 text-white rounded">Save</button>
        <button onClick={handleTest} disabled={isTesting} className="px-4 py-2 bg-gray-600 text-white rounded">Test SMTP</button>
      </div>
    </div>
  )
}
