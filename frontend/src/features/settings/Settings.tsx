import React, { useEffect, useState } from 'react'
import { useGetSettingsQuery, useUpdateSettingsMutation, useTestSmtpMutation } from './api/settingsApiSlice'

export default function Settings() {
  const { data, isLoading, error } = useGetSettingsQuery()
  const [updateSettings, { isLoading: isSaving }] = useUpdateSettingsMutation()
  const [testSmtp, { isLoading: isTesting }] = useTestSmtpMutation()

  const [labName, setLabName] = useState('')
  const [labSlogan, setLabSlogan] = useState('')
  const [contactEmail, setContactEmail] = useState('')
  const [address, setAddress] = useState('')
  const [phone, setPhone] = useState('')
  const [logoUrl, setLogoUrl] = useState<string | null>(null)

  const [smtpHost, setSmtpHost] = useState('')
  const [smtpPort, setSmtpPort] = useState(25)
  const [smtpUsername, setSmtpUsername] = useState('')
  const [smtpPassword, setSmtpPassword] = useState('')
  const [smtpUseTls, setSmtpUseTls] = useState(false)

  useEffect(() => {
    if (data) {
      setLabName(data.identity.labName)
      setLabSlogan(data.identity.labSlogan)
      setContactEmail(data.identity.contactEmail)
      setAddress(data.identity.address)
      setPhone(data.identity.phone)
      setLogoUrl(data.identity.logoUrl ?? null)
      setSmtpHost(data.smtp.host)
      setSmtpPort(data.smtp.port)
      setSmtpUsername(data.smtp.username)
      setSmtpUseTls(data.smtp.useTls)
    }
  }, [data])

  const handleSave = async () => {
    try {
      await updateSettings({
        identity: { labName, labSlogan, contactEmail, address, phone, logoUrl },
        smtp: { host: smtpHost, port: smtpPort, username: smtpUsername, password: smtpPassword || null, useTls: smtpUseTls },
      }).unwrap()
      alert('Saved successfully')
    } catch (err) {
      console.error(err)
      alert('Failed to save settings')
    }
  }

  const handleTest = async () => {
    try {
      await testSmtp({ testEmail: 'test@example.com' }).unwrap()
      alert('SMTP test successful')
    } catch (err) {
      console.error(err)
      alert('SMTP test failed')
    }
  }

  if (isLoading) return <div>Loading...</div>

  if (error) return <div>Error loading settings</div>

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">Lab Settings</h2>
      
      <div className="mb-4">
        <label className="block text-sm font-medium">Lab name</label>
        <input value={labName} onChange={(e) => setLabName(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">Lab slogan</label>
        <input value={labSlogan} onChange={(e) => setLabSlogan(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">Contact email</label>
        <input type="email" value={contactEmail} onChange={(e) => setContactEmail(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">Address</label>
        <input value={address} onChange={(e) => setAddress(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">Phone</label>
        <input value={phone} onChange={(e) => setPhone(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <h3 className="text-lg font-bold mb-4">SMTP Settings</h3>
      
      <div className="mb-4">
        <label className="block text-sm font-medium">SMTP Host</label>
        <input value={smtpHost} onChange={(e) => setSmtpHost(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">SMTP Port</label>
        <input type="number" value={smtpPort} onChange={(e) => setSmtpPort(parseInt(e.target.value))} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">SMTP Username</label>
        <input value={smtpUsername} onChange={(e) => setSmtpUsername(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium">SMTP Password</label>
        <input type="password" value={smtpPassword} onChange={(e) => setSmtpPassword(e.target.value)} className="mt-1 p-2 border rounded w-full" />
      </div>

      <div className="mb-4">
        <label className="flex items-center">
          <input type="checkbox" checked={smtpUseTls} onChange={(e) => setSmtpUseTls(e.target.checked)} className="mr-2" />
          <span className="text-sm font-medium">Use TLS</span>
        </label>
      </div>

      <div className="flex gap-2">
        <button onClick={handleSave} disabled={isSaving} className="px-4 py-2 bg-blue-600 text-white rounded">
          {isSaving ? 'Saving...' : 'Save'}
        </button>
        <button onClick={handleTest} disabled={isTesting} className="px-4 py-2 bg-gray-600 text-white rounded">
          {isTesting ? 'Testing...' : 'Test SMTP'}
        </button>
      </div>
    </div>
  )
}
